using Database.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Services.RecruiterServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.RecruiterServices
{
    public class RecruiterApplicationUsageService : IRecruiterApplicationUsageService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public RecruiterApplicationUsageService(AppDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<int> GetNumberOfAvailableTaskForRecruiter(int RecruiterId)
        {
            int numberOfAvailableTasks = await _context.Recruiter_Application_Usages.Where(R => R.RecruiterId == RecruiterId)
                                                                                    .Select(R => R.AvailableApplicationsNumber)
                                                                                    .FirstOrDefaultAsync();

            return numberOfAvailableTasks;
        }

        public async Task<bool> IsRecruiterHasAvailableApplicationsInAccount(int RecruiterId)
        {
            int NumberOfRecruiterTaks= await _context.Recruiter_Application_Usages.Where(R=>R.RecruiterId==RecruiterId)
                                                                                    .Select(R=>R.AvailableApplicationsNumber) 
                                                                                    .FirstOrDefaultAsync();

            return NumberOfRecruiterTaks != 0;
        }

        public async Task<bool> DecreaseTheAvailableApplicationsInAccount(int RecruiterId)
        {
            int RowsAffect = await _context.Recruiter_Application_Usages.Where(R => R.RecruiterId == RecruiterId)
                                                                  .ExecuteUpdateAsync(s => s.SetProperty(e => e.AvailableApplicationsNumber, e => e.AvailableApplicationsNumber - 1));

            return RowsAffect > 0;
        }

        public async Task<bool> AddInitialRecruiterApplicationUsageInfo(int RecruiterId)
        {
            int NumbersOfInitialApplication = Convert.ToInt32(_configuration.GetSection("FreeApplications").Value);

            var NewApplicationUsage = new Recruiter_Application_Usage()
            {
                RecruiterId = RecruiterId,
                AvailableApplicationsNumber = NumbersOfInitialApplication,
                TotalApplications = 0
            };

            await _context.Recruiter_Application_Usages.AddAsync(NewApplicationUsage);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
