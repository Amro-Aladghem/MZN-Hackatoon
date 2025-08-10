using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class University
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? image { get; set; }

        ICollection<Recruiter> Recruiters { get; set; } = new List<Recruiter>();
        ICollection<Seeker> Seekers { get; set; } = new List<Seeker>(); 
    }
}
