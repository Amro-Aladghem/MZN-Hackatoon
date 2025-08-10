using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JopAppliaction
{
    public class ApplicationFullDetailsDTO
    {
        public int ApplicationId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime DateOfCreated { get; set; }
        public int NumberOfApplied { get;set; }
        public string JobLevelName { get; set; }
        public string JobTypeName { get; set; }
        public TimeSpan TimeToComplete { get; set; }
        public bool IsUnviersityApplication { get; set; }
        public int? UniversityId { get; set; }   
        public string TypeName { get; set; } 
        public string? FileLink { get; set; }
    }
}
