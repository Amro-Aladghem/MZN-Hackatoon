using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JopAppliaction
{
    public class AddApplicationDTO
    {
        public string Subject { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? File_Link { get; set; }
        public int Jop_LevelId { get; set; }
        public int ? Jop_TypeId { get; set; }
        public int ApplicationTypeId { get; set; } = 1; // Default to 1, which is the "Job Application" type
        public bool IsUniversityApplication { get; set; } = false;

        public int UniversityId;
        public TimeSpan TimeToComplete { get; set; }
    }
}
