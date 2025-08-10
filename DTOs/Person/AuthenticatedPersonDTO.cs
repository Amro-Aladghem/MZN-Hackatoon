using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Person
{
    public class AuthenticatedPersonDTO
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageURL { get; set; }
        public string UserType { get; set; }
        public string Email { get; set; }
        public int? CountryId { get; set; }
    }
}
