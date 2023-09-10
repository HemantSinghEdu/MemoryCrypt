using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace WebApiEntityFramework.Models
{
    public class EmployeeResponseDto
    {
        public string EmployeeId { get; set; }

        public string FirstName { get; set; }


        public string LastName { get; set; }

        public int Age { get; set; }

        public string EmailAddress { get; set; }
        public IEnumerable<AddressDto>? Addresses { get; set; }


        public static implicit operator EmployeeResponseDto(Employee employee)
        {
            return new EmployeeResponseDto
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Age = employee.Age,
                EmailAddress = employee.EmailAddress,
                Addresses = employee.Addresses.Select(addr => (AddressDto)addr).ToList()
            };
        }

        public static implicit operator Employee(EmployeeResponseDto employeeDto)
        {
            return new Employee
            {
                EmployeeId = employeeDto.EmployeeId,
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Age = employeeDto.Age,
                EmailAddress = employeeDto.EmailAddress
            };
        }

    }
}
