namespace _02.Authentication.Models;

public class User
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}