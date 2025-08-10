using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JobApplicationResult
{
    public class ApplicationResultsResponseDTO
    {
        public List<ApplicationResultListDTO>? ApplicationResults { get; set; }
        public int NextPage { get; set; }
        public int TotalCount { get; set; } = 0;
        public int NumberOfApplied = 0;
    }
}
