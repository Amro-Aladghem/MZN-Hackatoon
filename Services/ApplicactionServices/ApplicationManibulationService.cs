using Database.Entites;
using DTOs.JopAppliaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApplicactionServices
{
    public  class ApplicationManibulationService:IApplicationManibulate
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ApplicationManibulationService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ApplicationShortInfoDTO?> GetActiveAppliactionShortInfo(int RecruiterId)
        {
            ApplicationShortInfoDTO? application = await _context.Applications.Where(R => R.RecruiterId == RecruiterId && R.IsActive == true)
                                                                                          .OrderByDescending(R => R.Id)
                                                                                          .Select(R => new ApplicationShortInfoDTO()
                                                                                          {
                                                                                              ApplicationId = R.Id,
                                                                                              DateOfCreation = R.DateOfCreation.ToString("yyyy-MM-dd HH:mm"),
                                                                                              NumbersOfApplied = R.NumbersOfApplied,
                                                                                              Subject = R.Subject
                                                                                          })
                                                                                          .FirstOrDefaultAsync();

            return application;
        }

        public async Task<bool> IsApplicationHasMaximumNumber(int ApplicationId)
        {
            int CurrentNumbersOfApplied = await _context.Applications.Where(J => J.Id == ApplicationId)
                                                                           .Select(J => J.NumbersOfApplied)
                                                                           .FirstOrDefaultAsync();

            int MaximumSeekersAppliedNumber = Convert.ToInt32(_configuration.GetSection("MaximumSeekersAppliedNumber").Value);

            bool IsHasMaximumNumber = CurrentNumbersOfApplied == MaximumSeekersAppliedNumber;

            return IsHasMaximumNumber;
        }

        public async Task IncreaseNumberOfAppliedByOne(int ApplicationId)
        {
            await _context.Applications.Where(J => J.Id == ApplicationId).ExecuteUpdateAsync(sp => sp.SetProperty(p => p.NumbersOfApplied, p => p.NumbersOfApplied + 1));
        }

        public async Task<ApplicationFullDetailsDTO?> GetPublicApplicationDetails(int ApplicationId)
        {
            ApplicationFullDetailsDTO? applicationFullDetailsDTO = await _context.Applications.Where(J => J.Id == ApplicationId && J.IsActive == true)
                                                                                                      .Select(J => new ApplicationFullDetailsDTO()
                                                                                                      {
                                                                                                          ApplicationId = J.Id,
                                                                                                          Subject = J.Subject,
                                                                                                          Description = J.Description,
                                                                                                          DateOfCreated = J.DateOfCreation,
                                                                                                          JobLevelName = J.Job_Level.Name,
                                                                                                          JobTypeName = J.Job_Type.Name,
                                                                                                          NumberOfApplied = J.NumbersOfApplied,
                                                                                                          TimeToComplete = J.TimeToComplete,
                                                                                                          IsUnviersityApplication = J.UniversityId != null,
                                                                                                          UniversityId = J.UniversityId,
                                                                                                          TypeName=J.Job_Type.Name
                                                                                                      })
                                                                                                      .AsNoTracking()
                                                                                                      .FirstOrDefaultAsync();

            return applicationFullDetailsDTO;
        }

        private IQueryable<Application> GetFilterQueryForListingJobApplicationsCards(ApplicationsListRequestDTO requestDTO)
        {
            IQueryable<Application> query = _context.Applications.Where(J => J.Id < requestDTO.LastApplicationId && J.IsActive);

            if (requestDTO.JobLevelId != 0 && requestDTO.JobLevelId is not null)
            {
                query = query.Where(J => J.Job_LevelId == requestDTO.JobLevelId);
            }

            if (requestDTO.JobTypeId != 0 && requestDTO.JobTypeId is not null)
            {
                query = query.Where(J => J.Job_TypeId == requestDTO.JobTypeId);
            }

            if (requestDTO.CountryId != 0 && requestDTO.CountryId is not null)
            {
                query = query.Where(J => J.Recruiter.Person.CountryId == requestDTO.CountryId);
            }

            if(requestDTO.UniversityId !=0 && requestDTO.UniversityId is not null)
            {
                query = query.Where(J => J.UniversityId == requestDTO.UniversityId);
            }

            return query;
        }

        public async Task<ApplicationsCardsListResponseDTO> GetApplicationsCardsForListing(ApplicationsListRequestDTO requestDTO)
        {
            IQueryable<Application> filterdQuery = GetFilterQueryForListingJobApplicationsCards(requestDTO);

            List<ApplicationListCardDTO>? applications = await filterdQuery.OrderByDescending(J => J.Id)
                                                                              .Take(requestDTO.Limit)
                                                                              .Select(J => new ApplicationListCardDTO()
                                                                              {
                                                                                  ApplicationId = J.Id,
                                                                                  JobLevelName = J.Job_Level.Name,
                                                                                  JobTypeName = J.Job_Type.Name,
                                                                                  RecruiterName = J.Recruiter.Person.FirstName + " " + J.Recruiter.Person.LastName,
                                                                                  RecruiterImage = J.Recruiter.Person.ImageURL,
                                                                                  CountryName = J.Recruiter.Person.Country.Name,
                                                                                  ShortDescription = J.Description.Length > 200 ? J.Description.Substring(0, 70) : J.Description,
                                                                                  Subject = J.Subject,
                                                                                  DateOfCreation = J.DateOfCreation,
                                                                                  UniversityName = J.UniversityId!= null ? J.University.Name : null,
                                                                                  IsUniversityApplication = J.UniversityId != null ?true : false
                                                                              })
                                                                              .ToListAsync();

            int LastJobApplicationId = requestDTO.LastApplicationId;

            if (applications.Count != 0)
            {
                LastJobApplicationId = applications[applications.Count - 1].ApplicationId;
            }

            return new ApplicationsCardsListResponseDTO()
            {
                applications = applications,
                LastJobApplicationId = LastJobApplicationId
            };
        }

        public async Task<int> GetNumbersOfAppliedSeekersForTheApplication(int ApplicationId)
        {
            int SeekersCount = await _context.Applications.Where(J => J.Id == ApplicationId)
                                                              .Select(J => J.NumbersOfApplied)
                                                              .FirstOrDefaultAsync();

            return SeekersCount;
        }

        public async Task<bool> IsApplicationForThisRecruiter(int RecruiterId, int ApplicationId)
        {
            var result = await _context.Applications.Where(J => J.RecruiterId == RecruiterId && J.Id == ApplicationId).AnyAsync();
            return result;
        }

        public async Task<bool> AddNewApplication(AddApplicationDTO addApplicationJopDTO, int RecruiterId, string? AiJobSummary = null)
        {
            var NewApplication = new Application()
            {
                Subject = addApplicationJopDTO.Subject,
                Description = addApplicationJopDTO.Description,
                DateOfCreation = DateTime.Now,
                File_Link = addApplicationJopDTO.File_Link,
                Job_LevelId = addApplicationJopDTO.Jop_LevelId,
                Job_TypeId = addApplicationJopDTO.Jop_TypeId,
                RecruiterId = RecruiterId,
                TimeToComplete = addApplicationJopDTO.TimeToComplete,
                AiSummary = AiJobSummary,
                IsActive = true,
                ApplicationTypeId=addApplicationJopDTO.ApplicationTypeId
            };

            if (addApplicationJopDTO.IsUniversityApplication)
            {
                NewApplication.UniversityId = addApplicationJopDTO.UniversityId;
            }

            await _context.Applications.AddAsync(NewApplication);
            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<List<ApplicationShortInfoDTO>?> GetApplicationsPaginatedDescending(IQueryable<Application?> BaseQuery, ApplicationsPaginatedRequestDTO requestDTO
            , int LastApplicationId)
        {
            List<ApplicationShortInfoDTO>? applications = await BaseQuery.Where(A => A.Id < LastApplicationId)
                                                                         .OrderByDescending(R => R.Id)
                                                                         .Take(requestDTO.Limit)
                                                                         .Select(A => new ApplicationShortInfoDTO()
                                                                         {
                                                                             ApplicationId = A.Id,
                                                                             DateOfCreation = A.DateOfCreation.ToString("yyyy-MM-dd HH:mm"),
                                                                             NumbersOfApplied = A.NumbersOfApplied,
                                                                             Subject = A.Subject,
                                                                             IsActive = A.IsActive,
                                                                             ShortDescription = A.Description.Length > 30 ? A.Description.Substring(0, 30) : A.Description
                                                                         })
                                                                         .ToListAsync();

            return applications;
        }

        public async Task<AppliactionsPaginatedDTO> GetApplicationsShortInfo(ApplicationsPaginatedRequestDTO requestDTO, int RecruiterId)
        {
            var BaseQuery = _context.Applications.Where(A => A.RecruiterId == RecruiterId);

            List<ApplicationShortInfoDTO>? applications = await GetApplicationsPaginatedDescending(BaseQuery, requestDTO, requestDTO.LastApplicationId);

            int RowsCount = 0;
            int LastApplicationId = requestDTO.LastApplicationId;

            if (!requestDTO.IsRowsCountCalculated)
            {
                RowsCount = await BaseQuery.CountAsync();
            }

            if (applications.Count != 0)
            {
                LastApplicationId = applications[applications.Count - 1].ApplicationId;
            }


            return new AppliactionsPaginatedDTO()
            {
                Applications = applications,
                Total = RowsCount,
                LastApplicationId = LastApplicationId,
            };
        }

        public bool IsApplicationDataCorrect(AddApplicationDTO addApplicationDTO)
        {
            if (string.IsNullOrEmpty(addApplicationDTO.Subject) || string.IsNullOrEmpty(addApplicationDTO.Description))
                return false;

            if (addApplicationDTO.Jop_LevelId <= 0 || addApplicationDTO.Jop_TypeId <= 0)
                return false;

            if (addApplicationDTO.TimeToComplete == TimeSpan.Zero)
                return false;

            return true;
        }

        public bool IsSeekerHasPermisionToJoin(string IsHasUniversity, string UniversityId, int ApplicationUniversityId)
        {
            if (!Convert.ToBoolean(IsHasUniversity))
                return false;

            if (!int.TryParse(UniversityId, out int universityId))
                return false;

            return universityId == ApplicationUniversityId;
        }
        
    }
}
