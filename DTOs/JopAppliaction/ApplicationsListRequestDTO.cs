using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JopAppliaction
{
    public class ApplicationsListRequestDTO
    {
        public int LastApplicationId { get; set; }
        public int Limit { get; set; }
        public int? JobLevelId { get; set; }
        public int? JobTypeId { get; set; }
        public int? CountryId { get; set; }
        public int? UniversityId { get; set; }
    }
}
