using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JopAppliaction
{
    public class JobApplicationStartResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ApplicationFullDetailsDTO? JobApplication { get; set; }
    }
}
