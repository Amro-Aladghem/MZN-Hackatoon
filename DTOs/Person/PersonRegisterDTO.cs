using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Person
{
    public class PersonRegisterDTO
    {
        public int ? PersonId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;   
        public string PhoneNumber { get; set; } = null!;
        public string? ImageURL { get;set; } 
        public int CountryId { get; set; }
        public int ?GovernorateId { get; set; }
        public string UserType { get; set; }
        public bool IsHasUniversity { get; set; } = false;
        public int? UniversityId { get; set; }
        
    }
}
