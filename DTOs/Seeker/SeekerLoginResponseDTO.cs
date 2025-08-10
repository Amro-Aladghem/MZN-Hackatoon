using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Seeker
{
    public class SeekerLoginResponseDTO
    {
        public int SeekerId { get; set; }
        public string FullName { get; set; }
        public string ImageURL { get; set; }
        public string UserType { get; set; }
        public bool IsProfileCompleted { get; set; }
        public int? CountryId { get; set; }
        public bool IsHasUniversity { get; set; } = false;
        public int? UniversityId { get; set; }
    }
}
