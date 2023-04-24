namespace MvcClient.Models;
public class RefreshRequest
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}