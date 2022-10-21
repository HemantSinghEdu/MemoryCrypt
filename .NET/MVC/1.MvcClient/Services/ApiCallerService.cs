using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MvcClient.Models;

namespace MvcClient.Services;

public class ApiCallerService : IApiCallerService
{
    private HttpClient _httpClient;
    private IConfiguration _config;

    public ApiCallerService(HttpClient httpClient, IConfiguration config)
    {
        //http client is a singleton
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<RegisterResponse> RegisterUser(RegisterRequest request)
    {
        var url = _config["apiService:userRegisterUrl"];
        var httpResponse = await MakeHttpCallAsync(
            httpMethod: HttpMethod.Post,
            url: url,
            bodyContent: request);

        var response = new RegisterResponse {
            Status = httpResponse.StatusCode
        };
        
        if(httpResponse.StatusCode != HttpStatusCode.OK)
        {
            //add error message
            response.Message = await httpResponse.Content.ReadAsStringAsync();
        }

        return response;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var url = _config["apiService:userLoginUrl"];
        var httpResponse = await MakeHttpCallAsync(
            httpMethod: HttpMethod.Post,
            url: url,
            bodyContent: request);
        
        var response = await httpResponse.Content.ReadFromJsonAsync<LoginResponse>();
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
            if(bodyContent != null)
            {
                var content = JsonSerializer.Serialize(bodyContent);
                httpRequest.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(httpRequest);

            return response;
        }
    }
}