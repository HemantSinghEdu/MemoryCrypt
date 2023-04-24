using System.ComponentModel.DataAnnotations;

namespace _02.Authentication.Models;

public class RevokeRequest
{
    [Required]
    public string RefreshToken { get; set; }
}