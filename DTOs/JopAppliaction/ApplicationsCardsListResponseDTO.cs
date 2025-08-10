using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JopAppliaction
{
    public class ApplicationsCardsListResponseDTO
    {
        public List<ApplicationListCardDTO> applications { get; set; }
        public int LastJobApplicationId { get; set; }
    }
}
