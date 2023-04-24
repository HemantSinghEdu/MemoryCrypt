using System.ComponentModel.DataAnnotations;

namespace CustomAuthentication.Models;

public class RevokeRequest
{
    [Required]
    public string RefreshToken { get; set; }
}