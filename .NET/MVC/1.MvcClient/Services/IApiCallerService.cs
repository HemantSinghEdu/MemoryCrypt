using MvcClient.Models;

namespace MvcClient.Services;

public interface IApiCallerService
{
    Task<RegisterResponse> RegisterUser(RegisterRequest request);
    Task<LoginResponse> Login(LoginRequest request);
    
}