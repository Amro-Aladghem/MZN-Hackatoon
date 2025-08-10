using DTOs.ApplicationResultQuestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JobApplicationResult
{
    public class ApplicationResultSumbitResponseDTO
    {
        public int Percentage { get; set; }
        public List<ApplicationResultQuestionDTO> Questions { get; set; }
    }
}
