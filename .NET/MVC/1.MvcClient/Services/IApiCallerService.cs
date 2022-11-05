using MvcClient.Models;

namespace MvcClient.Services;

public interface IApiCallerService
{
   Task<HttpResponseMessage> MakeHttpCallAsync(
        HttpMethod httpMethod,
        string url,
        object bodyContent = null,
        string acceptHeader = "application/json",
        string authScheme = null,
        string authToken = null,
        Dictionary<string, string> extraHeaders = null);
}