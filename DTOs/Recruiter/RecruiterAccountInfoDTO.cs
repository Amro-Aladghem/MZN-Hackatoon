using DTOs.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Recruiter
{
    public class RecruiterAccountInfoDTO
    {
        public int RecruiterId { get; set; }
        public int PersonId { get; set; }
        public string CompanyName { get; set; }
        public bool IsFreelancer { get; set; }
        public bool IsProfileComplete { get; set; }
        public string? LinkedIn { get; set; }
        public PersonAccountInfoDTO Person { get; set; }
    }
}
