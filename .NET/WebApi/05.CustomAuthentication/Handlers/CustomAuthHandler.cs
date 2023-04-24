using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using CustomAuthentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace CustomAuthentication.Handlers;

public class CustomAuthHandler
        : AuthenticationHandler<CustomAuthenticationSchemeOptions>
{
    private IConfiguration _config;
    private  UserManager<User> _userManager;
    public CustomAuthHandler(
        IOptionsMonitor<CustomAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration config,
        UserManager<User> userManager
        )
        : base(options, logger, encoder, clock)
    {
        _userManager = userManager;
        _config = config;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                return AuthenticateResult.Fail("Header Not Found.");
            }

            var header = Request.Headers[HeaderNames.Authorization].ToString();

            string tokenString = header.Substring("bearer".Length).Trim();

            var key = _config["token:key"];
            var tokenHandler = new CustomSecurityTokenHandler();
            
            var token = tokenHandler.GetDecryptedToken(tokenString, key);
            var email = token.Claims["Email"];
            
            var isTokenExpired = token.ValidTo < DateTime.Now;
            var userNotExist = await _userManager.FindByEmailAsync(email) == null;
            
            if(isTokenExpired || userNotExist)
            {
                return AuthenticateResult.Fail($"Unauthorized");               
            }  

            //if user is authenticated, control will reach here
            var principal = tokenHandler.GetPrincipalFromToken(tokenString, _config);

            // generate AuthenticationTicket from the Identity
            // and current authentication scheme
            var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

            // pass on the ticket to the middleware
            return AuthenticateResult.Success(ticket);
        }
        catch(Exception ex)
        {
            return AuthenticateResult.Fail($"Unauthorized: {ex.Message}");
        }
    }
}