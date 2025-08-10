using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JopAppliaction
{
    public class ApplicationListCardDTO
    {
        public int ApplicationId { get; set; }
        public string JobTypeName { get;set; }
        public string Subject { get; set; }
        public string ShortDescription { get; set; }
        public string JobLevelName { get; set; }
        public string RecruiterImage { get; set; }
        public string RecruiterName { get;  set; }
        public string CountryName { get; set; }
        public bool IsUniversityApplication { get; set; } = false;
        public string? UniversityName { get; set; }  
        public DateTime DateOfCreation { get; set; }
    }
}
