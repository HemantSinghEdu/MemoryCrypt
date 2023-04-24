using System.ComponentModel.DataAnnotations;

namespace _02.Authentication.Models;

public class LoginRequest
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
}