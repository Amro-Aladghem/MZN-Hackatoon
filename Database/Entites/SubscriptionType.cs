using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class SubscriptionType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        private int NumberOfTasks { get; set; }

        public ICollection<Application_Task_Payment> application_Task_Payments { get; set; }
    }
}
