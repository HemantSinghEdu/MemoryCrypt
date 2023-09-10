using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace WebApiEntityFramework.Models
{
    public class AddressDto
    {
        public string AddressType { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public int PostalCode { get; set; } 
        public string Country { get; set; }

        public static implicit operator Address(AddressDto addressDto)
        {
            return new Address
            {
                AddressType = addressDto.AddressType,
                AddressLine1 = addressDto.AddressLine1,
                AddressLine2 = addressDto.AddressLine2,
                City = addressDto.City,
                State = addressDto.State,
                PostalCode = addressDto.PostalCode,
                Country = addressDto.Country
            };
        }

        public static implicit operator AddressDto(Address address)
        {
            return new AddressDto
            {
                AddressType = address.AddressType,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country
            };
        }

    }
}
