using Microsoft.EntityFrameworkCore;
using WebApiEntityFramework.Models;

namespace WebApiEntityFramework.DatabaseContext
{
    public class InMemoryDbContext : DbContext
    {

        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "EmployeeDb");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //add unique index
            modelBuilder.Entity<Employee>()
                .HasIndex(e => new { e.FirstName, e.LastName, e.EmailAddress })
                .IsUnique(true);
        }
    }
}
