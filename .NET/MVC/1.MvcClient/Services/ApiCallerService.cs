using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using MvcClient.Models;

namespace MvcClient.Services;

public class ApiCallerService : IApiCallerService
{
    private HttpClient _httpClient;
    private IConfiguration _config;
    private IHttpContextAccessor _contextAccessor;

    public ApiCallerService(HttpClient httpClient, IConfiguration config, IHttpContextAccessor contextAccessor)
    {
        //http client is a singleton
        _httpClient = httpClient;
        _config = config;
        _contextAccessor = contextAccessor;
    }

    public async Task<RegisterResponse> RequestRegisterAsync(RegisterRequest request)
    {
        var url = _config["apiService:userRegisterUrl"];
        var httpResponse = await MakeHttpCallAsync(
            httpMethod: HttpMethod.Post,
            url: url,
            bodyContent: request);

        var response = new RegisterResponse
        {
            Status = httpResponse.StatusCode
        };

        if (httpResponse.StatusCode != HttpStatusCode.OK)
        {
            //add error message
            response.Message = await httpResponse.Content.ReadAsStringAsync();
        }

        return response;
    }

    public async Task<LoginResponse> RequestLoginAsync(LoginRequest request)
    {
        var url = _config["apiService:userLoginUrl"];
        var httpResponse = await MakeHttpCallAsync(
            httpMethod: HttpMethod.Post,
            url: url,
            bodyContent: request);
        LoginResponse loginResponse = new LoginResponse();
        
        //if login was successful
        if (httpResponse.StatusCode == HttpStatusCode.OK)
        {
            //map the login response
            loginResponse = await httpResponse.Content.ReadFromJsonAsync<LoginResponse>();
            loginResponse.Status = httpResponse.StatusCode;
            loginResponse.Email = request.Email;
        }
        else
        {
            //else if login failed, map the error message
            var errMessage = await httpResponse.Content.ReadAsStringAsync();
            loginResponse.Status = httpResponse.StatusCode;
            loginResponse.Message = errMessage;
        }
        return loginResponse;
    }


    public async Task<ArticlesResponse> RequestArticlesAsync()
    {
        var url = _config["apiService:articlesUrl"];
        var httpResponse = await MakeSecureHttpCallAsync(
            httpMethod: HttpMethod.Get,
            url: url
        );
        ArticlesResponse response = new ArticlesResponse();
        if (httpResponse.StatusCode == HttpStatusCode.OK)
        {
            var articles = await httpResponse.Content.ReadFromJsonAsync<List<Article>>();
            response.Status = httpResponse.StatusCode;
            response.Articles = articles;
        }
        else
        {
            response.Status = httpResponse.StatusCode;
            response.Message = await httpResponse.Content.ReadAsStringAsync();
        }
        return response;
    }

    private async Task<HttpResponseMessage> MakeSecureHttpCallAsync(
        HttpMethod httpMethod,
        string url,
        object bodyContent = null,
        string acceptHeader = "application/json",
        Dictionary<string, string> extraHeaders = null
    )
    {

        HttpResponseMessage httpResponse = null;
        var token = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "token")?.Value;
        if (!string.IsNullOrEmpty(token))
        {
            httpResponse = await MakeHttpCallAsync(
                httpMethod: HttpMethod.Get,
                url: url,
                authScheme: "bearer",
                authToken: token);
        }
        else
        {
            httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("Unauthorized")
            };
        }
        return httpResponse;
    }

    public async Task<LoginResponse> RequestRefreshAsync()
    {
        var url = _config["apiService:tokenRefreshUrl"];

        var email = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var token = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "token")?.Value;
        var refreshToken = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "refresh")?.Value;

        LoginResponse response = null;
        if (!(string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken)))
        {
            var refreshRequest = new RefreshRequest
            {
                AccessToken = token,
                RefreshToken = refreshToken
            };

            var httpResponse = await MakeHttpCallAsync(
            httpMethod: HttpMethod.Post,
            url: url,
            bodyContent: refreshRequest);
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                response = await httpResponse.Content.ReadFromJsonAsync<LoginResponse>();
                response.Status = httpResponse.StatusCode;
                response.Email = email;
            }
            else
            {
                response = new LoginResponse
                {
                    Status = httpResponse.StatusCode,
                    Message = await httpResponse.Content.ReadAsStringAsync()
                };

            }
        }
        return response;
    }

    private async Task<HttpResponseMessage> MakeHttpCallAsync(
        HttpMethod httpMethod,
        string url,
        object bodyContent = null,
        string acceptHeader = "application/json",
        string authScheme = null,
        string authToken = null,
        Dictionary<string, string> extraHeaders = null)
    {
        using (var httpRequest = new HttpRequestMessage(httpMethod, url))
        {
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));

            //add auth token if provided
            if (!string.IsNullOrEmpty(authToken) && !string.IsNullOrEmpty(authScheme))
            {
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue(authScheme, authToken);
            }

            //add any additional headers
            if (extraHeaders != null)
            {
                foreach (var header in extraHeaders)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }

            //add body if provided
            if (bodyContent != null)
            {
                var content = JsonSerializer.Serialize(bodyContent);
                httpRequest.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(httpRequest);

            return response;
        }
    }
}