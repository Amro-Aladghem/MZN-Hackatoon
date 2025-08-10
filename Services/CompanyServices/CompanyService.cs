using Database.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CompanyServices
{
    public class CompanyService
    {
        private readonly AppDbContext _context;

        public CompanyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int?> AddInitialNewCompanyInfo(string CompanyName)
        {
            var NewCompany = new Company()
            { 
               Name = CompanyName,
            };

            _context.Companies.Add(NewCompany);

            if (await _context.SaveChangesAsync() <= 0)
                return null;

            return NewCompany.Id;
        }

    }

}
