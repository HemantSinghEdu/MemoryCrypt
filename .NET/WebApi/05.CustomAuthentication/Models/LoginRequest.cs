using System.ComponentModel.DataAnnotations;

namespace CustomAuthentication.Models;

public class LoginRequest
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
}