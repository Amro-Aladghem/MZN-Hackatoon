using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class Recruiter_Application_Usage
    {
        [Key]
        public int Id { get; set; }

        public int RecruiterId { get; set; }

        public int AvailableApplicationsNumber { get; set; }
        public int TotalApplications { get; set; }

        public Recruiter Recruiter {get; set; }
    }
}
