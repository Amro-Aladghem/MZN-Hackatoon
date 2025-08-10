using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.JopApplicationOffer
{
    public class ApplicationOfferDTO
    {
        public int OfferId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal NumberOfTasks { get; set; }
    }
}
