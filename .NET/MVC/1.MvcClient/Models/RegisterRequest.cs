using System.ComponentModel.DataAnnotations;

namespace MvcClient.Models;

public class RegisterRequest
{
    [Required]
    [EmailAddress] 
    public string Email { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 1)]  
    public string Username { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [StringLength(50, MinimumLength = 8)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    [Display(Name="Confirm Password")]
    public string ConfirmPassword { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    [Display(Name="Display Name")]
    public string DisplayName { get; set; }
}