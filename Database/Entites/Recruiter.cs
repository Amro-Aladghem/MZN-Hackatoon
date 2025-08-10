using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class Recruiter
    {
        [Key]
        public int Id {  get; set; }
        public int PersonId { get; set; }
        public int? CompanyId { get; set; }
        public string? Linked_In_Link { get; set; } = null!;
        public DateTime? LastLoggedInTime { get; set; }
        public bool IsFreelancer { get; set; }
        public bool IsProfileComplete { get; set; }
        public bool IsInstructor { get; set; } = false;
        public int? UniversityId { get; set; }
        public Person Person { get; set; } = null!;
        public Company Company { get; set; } = null!;
        public University University { get; set; }
        public ICollection<Application> Job_Applications { get; set; }
        public Recruiter_Application_Usage Recruiter_Application_Usage { get; set; }
    }
}
