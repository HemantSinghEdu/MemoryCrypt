using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityFrameworkDbFirst.Models;

public class AuthDbContext : IdentityDbContext<User>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {

    }
}