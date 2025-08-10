using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JopAppliaction
{
    public class ApplicationsPaginatedRequestDTO
    {
        public int LastApplicationId { get; set; }
        public int Limit { get; set; }
        public bool IsRowsCountCalculated { get; set; }
    }
}
