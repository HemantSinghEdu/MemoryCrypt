using System.ComponentModel.DataAnnotations;

namespace IdentityFrameworkDbFirst.Models;

public class RefreshRequest
{
    [Required]
    public string AccessToken { get; set; }
    [Required]
    public string RefreshToken { get; set; }
}