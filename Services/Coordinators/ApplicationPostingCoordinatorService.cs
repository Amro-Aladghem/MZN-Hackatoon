using Database.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.ApplicactionServices;
using Services.RecruiterServices.Interfaces;
using DTOs.JopAppliaction;
using Services.RecruiterServices;

namespace Services.Coordinators
{
    public class ApplicationPostingCoordinatorService
    {
        private readonly AppDbContext _context;
        private readonly IApplicationManibulate _applicationManibulationService;
        private readonly IRecruiterApplicationUsageService _usageServices;

        public ApplicationPostingCoordinatorService(AppDbContext context, IApplicationManibulate applicationManibulationService
            , IRecruiterApplicationUsageService usageServices)
        {
            _context = context;
            _usageServices = usageServices;
            _applicationManibulationService= applicationManibulationService;
        }

        public async Task<bool> AddNewJopApplication(AddApplicationDTO addApplicationJopDTO,int RecruiterId,string? AiJobSummary)
        {
            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
                bool IsAddingDone = await _applicationManibulationService.AddNewApplication(addApplicationJopDTO,RecruiterId,AiJobSummary);

                bool IsDone = await _usageServices.DecreaseTheAvailableApplicationsInAccount(RecruiterId);

                if (!IsDone || !IsAddingDone)
                    throw new Exception("Faild to complete operation");

                transaction.Commit();
                return true;
            }
        }
    }
}
