using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CustomAuthentication.Handlers;
using CustomAuthentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CustomAuthentication.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    public IConfiguration _configuration;
    private UserManager<User> _userManager;

    public AuthController(IConfiguration config, UserManager<User> userManager)
    {
        _configuration = config;
        _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequestErrorMessages();
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        var isAuthorized = user != null && await _userManager.CheckPasswordAsync(user, request.Password);

        if (isAuthorized)
        {
            var authResponse = await GetTokens(user);
            user.RefreshToken = authResponse.RefreshToken;
            await _userManager.UpdateAsync(user);
            return Ok(authResponse);
        }
        else
        {
            return Unauthorized("Invalid credentials");
        }

    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequestErrorMessages();
        }

        //fetch email from expired token string
        var customTokenHandler = new CustomSecurityTokenHandler();
        var principal = customTokenHandler.GetPrincipalFromToken(request.AccessToken, _configuration);
        var userEmail = principal.FindFirstValue("Email"); //fetch the email claim's value

        //check if any user with email id has matching refresh token
        var user = !string.IsNullOrEmpty(userEmail) ? await _userManager.FindByEmailAsync(userEmail) : null;
        if (user == null || user.RefreshToken != request.RefreshToken)
        {
            return BadRequest("Invalid refresh token");
        }

        //provide new access and refresh tokens
        var response = await GetTokens(user);
        user.RefreshToken = response.RefreshToken;
        await _userManager.UpdateAsync(user);
        return Ok(response);
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke(RevokeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequestErrorMessages();
        }

        //fetch email from claims of currently logged in user
        var userEmail = this.HttpContext.User.FindFirstValue("Email");

        //check if any user with email id has matching refresh token
        var user = !string.IsNullOrEmpty(userEmail) ? await _userManager.FindByEmailAsync(userEmail) : null;
        if (user == null || user.RefreshToken != request.RefreshToken)
        {
            return BadRequest("Invalid refresh token");
        }

        //remove refresh token 
        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        return Ok("Refresh token is revoked");
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequestErrorMessages();
        }

        var isEmailAlreadyRegistered = await _userManager.FindByEmailAsync(registerRequest.Email) != null;

        if (isEmailAlreadyRegistered)
        {
            return Conflict($"Email Id {registerRequest.Email} is already registered.");
        }

        var newUser = new User
        {
            Email = registerRequest.Email,
            UserName = registerRequest.Username,
            DisplayName = registerRequest.DisplayName,
        };

        await _userManager.CreateAsync(newUser, registerRequest.Password);

        return Ok("User created successfully");

    }

    private async Task<AuthResponse> GetTokens(User user)
    {
        var customTokenHandler = new CustomSecurityTokenHandler();
        var accessToken = customTokenHandler.GetAccessToken(user, _configuration);
        var tokenStr = customTokenHandler.GetEncryptedString(accessToken, _configuration["token:key"]);
        var refreshTokenStr = customTokenHandler.GetRefreshToken(_userManager);
        var authResponse = new AuthResponse { AccessToken = tokenStr, RefreshToken = refreshTokenStr };
        return await Task.FromResult(authResponse);
    }

    private IActionResult BadRequestErrorMessages()
    {
        var errMsgs = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
        return BadRequest(errMsgs);
    }
}