using DTOs.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Seeker
{
    public  class SeekerAccountInfoDTO
    {
        public int SeekerId { get; set; }
        public int PersonId { get; set; }
        public string JobTypeName { get; set; }
        public string JobLevelName { get; set; }
        public string? ResumeLink { get; set; } = null!;
        public string? LinkedInLink { get; set; }
        public bool IsProfileCompleted { get; set; }
        public PersonAccountInfoDTO Person { get; set; }

    }
}
