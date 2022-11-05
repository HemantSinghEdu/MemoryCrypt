using Microsoft.AspNetCore.Authentication.Cookies;
using MvcClient.Models;

namespace MvcClient.Services;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task LoginWithCookieAsync(string email, string accessToken, string refreshToken);
    Task<LoginResponse> RefreshTokenAsync(Claims claims = null);
    Task<bool> RevokeTokenAsync();    
    Task LogoutAsync();

    Claims GetSavedClaims();

    Task TakeActionIfTokenExpired(CookieValidatePrincipalContext context);

}