using System.ComponentModel.DataAnnotations;

namespace IdentityFrameworkDbFirst.Models;

public class LoginRequest
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
}