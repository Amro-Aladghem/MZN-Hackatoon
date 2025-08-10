using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Person> Persons { get; set; } = null!;
        public ICollection<Governorate> Governorates { get; set;} = null!;
        public ICollection<Company> Companies { get; set; } = null!;
        public Phone_Code Phone_Code { get; set; }
    }
}
