using DTOs.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Seeker
{
    public class SeekerRegisterDTO
    {
        public int JobLevelId { get; set; }
        public int JobTypeId { get; set; }
        public string? LinkeInLink { get; set; }
        public PersonRegisterDTO person { get; set; }
        public int? UniversityYear { get; set; }
        public string? UniversityNumberId { get; set; }
    }
}
