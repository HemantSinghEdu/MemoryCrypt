using System.Net;

namespace MvcClient.Models;

public class LoginResponse
{
    public HttpStatusCode Status { get; set; }
    public string Message { get; set; }
    public string Email { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}