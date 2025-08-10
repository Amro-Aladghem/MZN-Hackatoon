using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class Application_Result
    {
        [Key]
        public int Id { get; set; }
        public int SeekerId { get; set; }
        public int Application_Result_StatusId { get; set; }
        public TimeSpan? TakenTimeToComplete { get; set; }
        public decimal? Result { get; set; }
        public int ApplicationId { get; set; }
        public DateTime DateAndTimeOfJoined { get; set; }
        public string? SolutionFileUri { get; set; }
        

        public Application_Result_Status application_Result_Status { get; set; } = null!;
        public Seeker Seeker { get; set; } = null!;
        public Application Application { get; set; } = null!;

        public List<Application_Result_Question> questions { get; set; } = new List<Application_Result_Question>();
    }
}
