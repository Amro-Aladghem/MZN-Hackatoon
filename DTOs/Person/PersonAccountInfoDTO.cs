using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Person
{
    public class PersonAccountInfoDTO
    {
        public string FullName { get; set; }
        public string CountryName { get; set; }
        public string? GovernorateName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Image { get; set; }

    }
}
