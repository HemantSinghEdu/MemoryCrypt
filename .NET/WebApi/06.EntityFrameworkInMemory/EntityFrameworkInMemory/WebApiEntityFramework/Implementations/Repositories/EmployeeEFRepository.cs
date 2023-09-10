using Microsoft.EntityFrameworkCore;
using WebApiEntityFramework.Contracts.Repositories;
using WebApiEntityFramework.DatabaseContext;
using WebApiEntityFramework.Models;

namespace WebApiEntityFramework.Implementations.Repositories
{
    public class EmployeeEFRepository : EFRepository<Employee>, IEmployeeRepository
    {
        public EmployeeEFRepository(InMemoryDbContext context) : base(context)
        {
        }
        public override async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _dbSet.Include(e => e.Addresses).ToListAsync();
        }

        public override async Task<Employee> GetById(string id)
        {
            return await _dbSet.Include(e => e.Addresses).FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

    }
}
