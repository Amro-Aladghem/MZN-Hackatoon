using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class Application
    {
        [Key]
        public int Id { get; set; } 
        public string Subject { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? File_Link { get; set; }
        public int  RecruiterId { get; set; }
        public int Job_LevelId {  get; set; }
        public int? Job_TypeId { get; set; } // here it null ,if the application for Universisty type.
        public bool IsActive { get; set; }
        public TimeSpan TimeToComplete { get; set; }
        public DateTime DateOfCreation { get; set; }
        public int NumbersOfApplied { get; set; }
        public string? AiSummary { get; set; }
        public int ApplicationTypeId { get; set; }
        public int? UniversityId { get; set; }

        public Recruiter Recruiter { get; set; } = null!;
        public Job_Level Job_Level { get; set; } = null!;
        public ICollection<Application_Result> Application_Results { get; set; }
        public Job_Type Job_Type { get; set; }
        public ICollection<Application_Task_Payment> application_Task_Payments { get; set; }
        public University University { get; set; }
        public ApplicationType ApplicationType { get; set; }

    }
}
