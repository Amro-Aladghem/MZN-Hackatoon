using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JobApplicationResult
{
    public class ApplicationResultListDTO
    {
        public int ApplicationResultId { get; set; }
        public int SeekerId { get; set; }
        public string SeekerName { get; set; }
        public string SeekerImage {  get; set; }    
        public decimal ResultPercentage { get; set; }
        public string TimeToComplete { get; set; }
        public DateTime DateOfResult { get; set; }
        public string? SolutionFile { get; set; }
    }
}
