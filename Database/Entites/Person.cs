using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string ?FirstName { get; set; } 
        public string ?LastName { get; set; }
        public string ?ImageURL { get; set; }
        public string ? Google_Id { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public int? CountryId { get; set; }
        public int? GovernorateId { get; set; }
        public bool IsActive { get; set; }
        public int UserTypeId { get; set; }

        public Governorate Governorate { get; set; } = null!;
        public Country Country { get; set; } = null!;
        public Seeker Seeker { get; set; } 
        public Recruiter Recruiter { get; set; }
        public UserType UserType { get; set; }
    }
}
