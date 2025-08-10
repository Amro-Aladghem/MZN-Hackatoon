using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } 
        public string? ImageURL { get; set; }
        public int? CountryId { get; set; }
        public int? GovernorateId { get; set; }
        public string? Linked_In_Link { get; set; } = null!;

        public Country Country { get; set; } = null!;
        public Governorate Governorate { get; set; } = null!;
        public ICollection<Recruiter> Recruiters { get; set; } = null!;
    }
}
