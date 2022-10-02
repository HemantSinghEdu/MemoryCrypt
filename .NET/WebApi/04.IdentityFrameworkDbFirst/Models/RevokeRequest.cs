using System.ComponentModel.DataAnnotations;

namespace IdentityFrameworkDbFirst.Models;

public class RevokeRequest
{
    [Required]
    public string RefreshToken { get; set; }
}