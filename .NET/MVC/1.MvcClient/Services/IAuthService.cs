using MvcClient.Models;

namespace MvcClient.Services;

public interface IAuthService
{
    Task LoginAsync(string email, string accessToken, string refreshToken);
    Task LogoutAsync();
}