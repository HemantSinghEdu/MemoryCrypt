using System.ComponentModel.DataAnnotations;

namespace WebApiEntityFramework.Models
{
    public class Employee
    {
        [Key]
        public string EmployeeId { get; set; }  
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string EmailAddress { get; set; }

        //navigation property to reach addresses associated with this employee
        public ICollection<Address> Addresses { get; set; }

    }
}
