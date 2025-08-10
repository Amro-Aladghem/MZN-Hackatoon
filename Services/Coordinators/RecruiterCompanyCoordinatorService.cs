using Database.Entites;
using DTOs.Recruiter;
using Services.CompanyServices;
using Services.RecruiterServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Coordinators
{
    public class RecruiterCompanyCoordinatorService
    {
        private readonly AppDbContext _context;
        private readonly RecruiterService _recruiterServices;
        private readonly CompanyService _companyService;

        public RecruiterCompanyCoordinatorService(AppDbContext context,RecruiterService recruiterService,CompanyService companyService)
        {
            _context = context;
            _recruiterServices = recruiterService;
            _companyService = companyService;
        }

        public async Task<bool> HandleCompleteProfileInfoProcess(RecruiterCompleteProfileDTO completeProfileDTO,int RecruiterId)
        {
            int? newCompanyId = null;

            bool IsHasCompany = !string.IsNullOrEmpty(completeProfileDTO.CompanyName);

            using (var transaction =await _context.Database.BeginTransactionAsync())
            {
                if (IsHasCompany)
                {
                    newCompanyId = await _companyService.AddInitialNewCompanyInfo(completeProfileDTO.CompanyName!);

                    if (newCompanyId is null)
                        throw new Exception($"Faild to add company for recruiter id:${RecruiterId}");
                }

                bool IsRecruiterUpdated = await _recruiterServices.SetCompleteProfileInfo(completeProfileDTO, RecruiterId,newCompanyId);

                if(!IsRecruiterUpdated)
                    throw new Exception($"Faild to complete recruiter profile for recruiter id:${RecruiterId}");

                await transaction.CommitAsync();
                return true;
            }
        }
    }
}
