using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JobApplicationResult
{
    public class ApplicationResultSumbitInfoDTO
    {
        public int resultId { get; set; }
        public DateTime DateOfJoined { get; set; }
        public string SolutionFileUri { get; set; }
        public string ApplicationSummery { get; set; }
        public TimeSpan TimeToCompleteApplication { get; set; }
    }

}
