using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class Application_Task_Payment
    {
        [Key]
        public int Id { get; set; }
        public int UserTypeId { get; set; }
        public int UserId { get; set; }
        public decimal PaidAmount { get; set; }
        public DateOnly Date { get; set; }
        public DateTime Time { get; set; }
        public int? SubscriptionTypeId { get; set; }
        public int ? ApplicationId { get; set; }

        public SubscriptionType SubscriptionType { get; set; }
        public Application Application { get; set; }
    }
}
