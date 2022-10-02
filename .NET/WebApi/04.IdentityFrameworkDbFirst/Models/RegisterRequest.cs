using System.ComponentModel.DataAnnotations;

namespace IdentityFrameworkDbFirst.Models;

public class RegisterRequest
{
    [Required]
    [EmailAddress]  
    public string Email { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 1)]  
    public string Username { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 8)]
    public string Password { get; set; }

    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    [Required]
    public string DisplayName { get; set; }

}