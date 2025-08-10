using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class Seeker
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int? Job_TypeId { get; set; }
        public int ?Job_LevelId { get; set; }
        public string? ResumeLink { get; set; } = null!;
        public string? LinkedInLink { get; set; }
        public bool IsProfileCompleted { get; set; }
        public DateTime LastLoggedInTime { get; set; }
        public bool IsStudent { get; set; } = false;
        public int? UniversityId { get; set; }
        public string? UniversityStudentNumber { get; set; }

        [Range(1,6,ErrorMessage ="The number must be between 1 to 4")]
        public int? StudyingYear { get; set; }
        public Job_Level Job_Level { get; set; } = null!;
        public Job_Type Job_Type { get; set; } = null!;
        public Person Person { get; set; } = null!;
        public ICollection<Application_Result> Application_Results { get; set; }
        public University University { get; set; }
    }
}
