using Database.Entites;
using DTOs.JopApplicationOffer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.JobApplicationOfferService
{
    public class JobAppliactionOfferService
    {
        private readonly AppDbContext _context;

        public JobAppliactionOfferService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ApplicationOfferDTO>?> GetJopApplicationOffers()
        {
            List<ApplicationOfferDTO>? offers = await _context.Application_Offers
                                                                     .Select(O => new ApplicationOfferDTO()
                                                                     {
                                                                         OfferId = O.Id,
                                                                         Title = O.Title,
                                                                         Description = O.Description,
                                                                         NumberOfTasks=O.NumberOfTasks,
                                                                         Price = O.Price,
                                                                     })
                                                                     .ToListAsync();

            return offers;
        }

        public async Task<string?> GetOfferTitleIfExistsById(int OfferId)
        {
            string? title = await _context.Application_Offers.Where(O=>O.Id==OfferId).Select(O=>O.Title).FirstOrDefaultAsync();

            return title;
        }
    }
}
