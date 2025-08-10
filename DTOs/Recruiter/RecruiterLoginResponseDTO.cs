using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Recruiter
{
    public class RecruiterLoginResponseDTO
    {
        public int RecruiterId { get;set; }
        public string FullName { get; set; }
        public string ImageURL { get; set; }
        public string UserType { get; set; }
        public bool IsProfileCompleted { get; set; }
        public bool IsHasUniversity { get; set; }
        public int? UniversityId { get; set; }
    }
}
