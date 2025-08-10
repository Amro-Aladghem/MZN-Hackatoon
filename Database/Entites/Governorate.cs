using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class Governorate
    {
        [Key]
        public int Id { get; set; }
        public int Name { get; set; }
        public int CountryId { get; set; }

        public Country Country { get; set; } = null!;
        public ICollection<Company> Companies { get; set; } = null!;
        public ICollection<Person> Persons { get; set; }
    }
}
