using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JopAppliaction
{
    public class ApplicationShortInfoDTO
    {
        public int ApplicationId { get; set; }
        public string Subject { get; set; }
        public string ? ShortDescription { get; set; }
        public int NumbersOfApplied { get; set; }
        public string DateOfCreation { get; set; }
        public bool IsActive { get; set; }  
    }
}
