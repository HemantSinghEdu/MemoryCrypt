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

    public ApiCallerService(HttpClient httpClient)
    {
        //http client is a singleton
        _httpClient = httpClient;
    }


    public async Task<HttpResponseMessage> MakeHttpCallAsync(
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