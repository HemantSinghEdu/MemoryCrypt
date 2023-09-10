using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiEntityFramework.Models
{
    public class Address
    {
        [Key]
        public string AddressId { get; set; }  
        public string AddressType { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int PostalCode { get; set; } 
        public string Country { get; set; }

        [ForeignKey("Employee")]
        public string EmployeeId { get; set; }  //foreign key
        public Employee Employee { get; set; }  //navigation property to reach employee associated with this address
    }
}
