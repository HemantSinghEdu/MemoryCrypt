using System.ComponentModel.DataAnnotations;

namespace CustomAuthentication.Models;

public class RefreshRequest
{
    [Required]
    public string AccessToken { get; set; }
    [Required]
    public string RefreshToken { get; set; }
}