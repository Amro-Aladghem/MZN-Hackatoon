using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class Application_Result_Status
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<Application_Result> Application_Results { get; set; } = null!;
    }
}
