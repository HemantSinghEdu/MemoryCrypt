using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MvcClient.Models;

namespace MvcClient.Services;

public class AuthService : IAuthService
{
    private IApiCallerService _apiService;
    private IHttpContextAccessor _contextAccessor;
    private IConfiguration _config;
    public AuthService(IConfiguration config, IApiCallerService apiService, IHttpContextAccessor contextAccessor)
    {
        _config = config;
        _apiService = apiService;
        _contextAccessor = contextAccessor;
    }
    public async Task LoginWithCookieAsync(string email, string accessToken, string refreshToken)
    {
        //generate claims for email, access token, and refresh token
        var principal = GeneratePrincipal(email, accessToken, refreshToken);

        //create a cookie with above claims
        await _contextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    private ClaimsPrincipal GeneratePrincipal(string email, string accessToken, string refreshToken)
    {
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.Email, email));
        identity.AddClaim(new Claim("token", accessToken));
        identity.AddClaim(new Claim("refresh", refreshToken));

        var principal = new ClaimsPrincipal(identity);
        return principal;
    }

    public Claims GetSavedClaims()
    {
        var claimsList = _contextAccessor.HttpContext.User.Claims;
        var claimsObject = GenerateClaimsObject(claimsList);
        return claimsObject;
    }

    private Claims GenerateClaimsObject(IEnumerable<Claim> claims)
    {
        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var token = claims.FirstOrDefault(c => c.Type == "token")?.Value;
        var refreshToken = claims.FirstOrDefault(c => c.Type == "refresh")?.Value;

        return new Claims { Email = email, AuthToken = token, RefreshToken = refreshToken };

    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        var url = _config["apiService:userRegisterUrl"];
        var httpResponse = await _apiService.MakeHttpCallAsync(
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

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var url = _config["apiService:userLoginUrl"];
        var httpResponse = await _apiService.MakeHttpCallAsync(
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

    public async Task TakeActionIfTokenExpired(CookieValidatePrincipalContext context)
    {
        //get current claims from CookieValidatePrincipalContext
        var currentClaims = GenerateClaimsObject(context.Principal.Claims); //can't fetch from user claims yet as they are not saved
        var shouldRedirectToLogin = false;

        if (string.IsNullOrEmpty(currentClaims.AuthToken))
        {
            shouldRedirectToLogin = true;
        }
        else
        {
            //check if auth token is still valid
            var isTokenValid = await ValidateToken(currentClaims.AuthToken);
            if (!isTokenValid) 
            { 
                //refresh auth token 
                var refreshResponse = await RefreshTokenAsync(currentClaims);
                if(refreshResponse.Status != HttpStatusCode.OK)
                {
                    shouldRedirectToLogin = true;
                }
                else
                {
                    //update the principal (claims) in context object
                    var newPrincipal = GeneratePrincipal(refreshResponse.Email, refreshResponse.AccessToken, refreshResponse.RefreshToken);
                    context.ReplacePrincipal(newPrincipal);
                    context.ShouldRenew = true;
                }
            }
        }

        if(shouldRedirectToLogin)
        {
            //if for any reason tokens could not be refreshed
            //reject the principal and remove the current cookie  
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync();
        }
    }

    private async Task<bool> ValidateToken(string authToken)
    {
        var url = _config["apiService:tokenValidateUrl"];

        if (!(string.IsNullOrEmpty(authToken)))
        {
            var httpResponse = await _apiService.MakeHttpCallAsync(
                    httpMethod: HttpMethod.Post,
                    url: url,
                    authScheme: "bearer",
                    authToken: authToken);

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
        }

        return false;
    }

    public async Task<LoginResponse> RefreshTokenAsync(Claims claims = null)
    {
        var url = _config["apiService:tokenRefreshUrl"];

        var currentClaims = claims == null ? GetSavedClaims() : claims;

        LoginResponse response = null;
        if (!(string.IsNullOrEmpty(currentClaims.AuthToken) || string.IsNullOrEmpty(currentClaims.RefreshToken)))
        {
            var refreshRequest = new RefreshRequest
            {
                AccessToken = currentClaims.AuthToken,
                RefreshToken = currentClaims.RefreshToken
            };

            //make a call to web api to get new tokens
            var httpResponse = await _apiService.MakeHttpCallAsync(
            httpMethod: HttpMethod.Post,
            url: url,
            bodyContent: refreshRequest);

            //if call was successful
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                response = await httpResponse.Content.ReadFromJsonAsync<LoginResponse>();
                response.Status = httpResponse.StatusCode;
                response.Email = currentClaims.Email;
            }
            else
            {
                //if call failed
                response = new LoginResponse
                {
                    Status = httpResponse.StatusCode,
                    Message = await httpResponse.Content.ReadAsStringAsync()
                };

            }
        }
        return response;
    }

    public async Task<bool> RevokeTokenAsync()
    {
        var url = _config["apiService:tokenRevokeUrl"];

        var currentClaims = GetSavedClaims();

        if (!(string.IsNullOrEmpty(currentClaims.AuthToken) || string.IsNullOrEmpty(currentClaims.RefreshToken)))
        {
            var revokeRequest = new RevokeRequest
            {
                RefreshToken = currentClaims.RefreshToken
            };

            var httpResponse = await _apiService.MakeHttpCallAsync(
            httpMethod: HttpMethod.Post,
            url: url,
            bodyContent: revokeRequest,
            authScheme: "bearer",
            authToken: currentClaims.AuthToken);

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        await _contextAccessor.HttpContext.SignOutAsync();
    }

}