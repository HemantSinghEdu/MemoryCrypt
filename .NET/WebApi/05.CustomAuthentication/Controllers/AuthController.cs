using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        var userEmail = principal.FindFirstValue("Email"); //fetch the email claim's value
        
        //check if any user with email id has matching refresh token
        var user = !string.IsNullOrEmpty(userEmail) ? await _userManager.FindByEmailAsync(userEmail) : null;
        if(user==null || user.RefreshToken != request.RefreshToken)
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
        if( user == null || user.RefreshToken != request.RefreshToken)
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

    private string GetRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        //ensure token is unique by checking against db
        var tokenIsUnique = !_userManager.Users.Any(u => u.RefreshToken == token);

        if (!tokenIsUnique)
            return GetRefreshToken();  //recursive call
            
        return token;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["token:key"])),
            ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }

    private async Task<AuthResponse> GetTokens(User user)
    {
            //create claims details based on the user information
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["token:subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.Id),
                        new Claim("UserName", user.UserName),
                        new Claim("Email", user.Email)
                    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["token:key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["token:issuer"],
                _configuration["token:audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["token:accessTokenExpiryMinutes"])),
                signingCredentials: signIn);
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshTokenStr = GetRefreshToken();
            var authResponse = new AuthResponse { AccessToken = tokenStr, RefreshToken = refreshTokenStr };
            return await Task.FromResult(authResponse);
    }

    private IActionResult BadRequestErrorMessages()
    {
        var errMsgs = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
        return BadRequest(errMsgs);
    }
}