using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Entites;
using Services.RecruiterServices;
using Services.RecruiterServices.Interfaces;
using DTOs;
using DTOs.Person;
using DTOs.Recruiter;

namespace Services.Coordinators
{
    public class RecruiterLaunshCoordinatorService
    {
        private readonly AppDbContext _context;
        private readonly RecruiterService _recruiterService;
        private readonly IRecruiterApplicationUsageService _applicationUsageService;

        public RecruiterLaunshCoordinatorService(AppDbContext context,RecruiterService recruiterService,IRecruiterApplicationUsageService recruiterApplicationUsageService)
        {
            _context= context;
            _recruiterService = recruiterService;
            _applicationUsageService = recruiterApplicationUsageService;
        }

        public async Task<RecruiterLoginResponseDTO?> LaunshRecruiterAccount(PersonRegisterDTO personRegisterDTO,int PersonId)
        {
            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    RecruiterLoginResponseDTO? responseDTO =await _recruiterService.RegisterRecruiter(personRegisterDTO, PersonId);

                    if (responseDTO is null)
                        throw new Exception("Failed to register this person!");

                    bool isDone = await _applicationUsageService.AddInitialRecruiterApplicationUsageInfo(responseDTO.RecruiterId);

                    if(!isDone)
                        throw new Exception("Failed to register this person!");

                    await transaction.CommitAsync();

                    return responseDTO;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return null;
                }
            }
        }
    }
}
