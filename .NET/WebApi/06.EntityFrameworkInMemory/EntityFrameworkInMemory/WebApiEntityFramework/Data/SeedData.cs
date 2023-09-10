using Microsoft.EntityFrameworkCore;
using WebApiEntityFramework.Contracts.Repositories;
using WebApiEntityFramework.Models;

namespace WebApiEntityFramework.Data
{
    public static class SeedData
    {
        public static List<Employee> InitialEmployees = new List<Employee>
            {
                new Employee
                {
                    EmployeeId = Guid.NewGuid().ToString(),
                    FirstName = "John",
                    LastName = "Doe",
                    Age = 30,
                    EmailAddress = "john.doe@example.com"
                },
                new Employee
                {
                    EmployeeId = Guid.NewGuid().ToString(),
                    FirstName = "Jane",
                    LastName = "Walsh",
                    Age = 35,
                    EmailAddress = "jane.walsh@example.com"

                },
                new Employee
                {
                    EmployeeId = Guid.NewGuid().ToString(),
                    FirstName = "Prakash",
                    LastName = "Dubey",
                    Age = 25,
                    EmailAddress = "prakash.dubey@example.com"

                },
                new Employee
                {
                    EmployeeId = Guid.NewGuid().ToString(),
                    FirstName = "Arun",
                    LastName = "Nayar",
                    Age = 47,
                    EmailAddress = "arun.nayar@example.com"

                }
            };


        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var employeeRespository = serviceProvider.GetRequiredService<IEmployeeRepository>();

            // Look for existing records
            var employeeList = await employeeRespository.GetAllAsync();
            if (employeeList.Any())
            {
                return;   // DB has been seeded
            }

            //If no records exist yet, then seed the database
            await employeeRespository.CreateAsync(InitialEmployees);
        }
    }
}
