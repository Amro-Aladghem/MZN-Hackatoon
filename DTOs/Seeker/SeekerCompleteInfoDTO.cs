using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Seeker
{
    public class SeekerCompleteInfoDTO
    {
        public int CountryId { get; set; }
        public string Phone { get; set; }
        public int JobLevelId { get; set; }
        public int JobTypeId { get; set; }
        public string? ResumeLink { get; set; }
        public string? LinkedInLink { get; set; }   
        public bool IsStudent { get; set; }
        public int? UniversityId { get; set; }
        public int? UniversityYear { get; set; }
        public string? UniversityNumberId { get; set;}
        
    }
}
