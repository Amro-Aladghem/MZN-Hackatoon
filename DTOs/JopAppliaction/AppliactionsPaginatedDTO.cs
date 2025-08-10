using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JopAppliaction
{
    public class AppliactionsPaginatedDTO
    {
        public List<ApplicationShortInfoDTO>? Applications { get; set; }
        public int LastApplicationId { get; set; }
        public int Total { get; set; }
    }
}
