using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JobApplicationResult
{
    public class ApplicationResultsRequestDTO
    {
        public int Limit { get; set; }
        public int Page { get; set; }
        public bool IsRowCountCalculated { get; set; }  
    }
}
