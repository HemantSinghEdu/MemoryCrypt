using MvcClient.Models;

namespace MvcClient.Services;

public interface IApiCallerService
{
    Task<RegisterResponse> RequestRegisterAsync(RegisterRequest request);
    Task<LoginResponse> RequestLoginAsync(LoginRequest request);
    Task<LoginResponse> RequestRefreshAsync();
    Task<bool> RequestRevokeAsync();    
    Task<ArticlesResponse> RequestArticlesAsync();
    
}