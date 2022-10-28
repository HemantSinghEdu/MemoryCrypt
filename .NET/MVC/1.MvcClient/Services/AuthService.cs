using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MvcClient.Models;

namespace MvcClient.Services;

public class AuthService : IAuthService
{
    private IHttpContextAccessor _contextAccessor;
    public AuthService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
    public async Task LoginAsync(string email, string accessToken, string refreshToken)
    {
        //Instruct the browser to store the claims in a cookie 
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.Email, email));
        identity.AddClaim(new Claim("token", accessToken));
        identity.AddClaim(new Claim("refresh", refreshToken));

        var principal = new ClaimsPrincipal(identity);
        //create a cookie with above claims
        await _contextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    public async Task LogoutAsync()
    {
        await _contextAccessor.HttpContext.SignOutAsync();
    }
}