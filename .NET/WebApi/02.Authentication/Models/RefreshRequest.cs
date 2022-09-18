using System.ComponentModel.DataAnnotations;

namespace _02.Authentication.Models;

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; }
}