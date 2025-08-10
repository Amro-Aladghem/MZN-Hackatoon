using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class Job_Type
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<Seeker> Seekers { get; set; } = null!;
        public ICollection<Application> Job_Applications { get; set; }
    }
}

