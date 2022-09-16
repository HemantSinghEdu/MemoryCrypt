using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using _02.Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace _02.Authentication.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    public List<User> tempUserDb = new List<User>{
            new User{UserId="abc123",UserName="John", DisplayName="BilboBaggins", Email="john@abc.com", Password="john@123" },
            new User{UserId="def456",UserName="Jane", DisplayName="Galadriel", Email="jane@xyz.com", Password="jane1990" }
        };

    public IConfiguration _configuration;

    public AuthController(IConfiguration config)
    {
        _configuration = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequestErrorMessages();
        }

        var user = await GetUser(request.Email, request.Password);

        if (user != null)
        {
            //create claims details based on the user information
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["token:subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.UserId),
                        new Claim("DisplayName", user.DisplayName),
                        new Claim("UserName", user.UserName),
                        new Claim("Email", user.Email)
                    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["token:key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["token:issuer"],
                _configuration["token:audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            var authResponse = new AuthResponse { AccessToken = tokenStr };

            return Ok(authResponse);
        }
        else
        {
            return BadRequest("Invalid credentials");
        }

    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequestErrorMessages();
        }

        var isEmailAlreadyRegistered = await GetUserByEmail(registerRequest.Email) != null;

        if(isEmailAlreadyRegistered)
        {
            return Conflict($"Email Id {registerRequest.Email} is already registered.");
        }

        await AddUser(new User{ Email=registerRequest.Email, Password=registerRequest.Password });

        return Ok("User created successfully");

    }

    private async Task<User> GetUser(string email, string password)
    {
        return await Task.FromResult(tempUserDb.FirstOrDefault(u => u.Email == email && u.Password == password));
    }

    private async Task<User> GetUserByEmail(string email)
    {
        return await Task.FromResult(tempUserDb.FirstOrDefault(u => u.Email == email));
    }

    private async Task<User> AddUser(User newUser)
    {
        newUser.UserId = $"user{DateTime.Now.ToString("hhmmss")}";
        tempUserDb.Add(newUser);
        return newUser;
    }


    private IActionResult BadRequestErrorMessages()
    {
        var errMsgs = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
        return BadRequest(errMsgs);
    }
}