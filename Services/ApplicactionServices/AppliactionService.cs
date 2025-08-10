using Database.Entites;
using DTOs.JopAppliaction;
using DTOs.JopApplicationOffer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApplicactionServices
{
    public class AppliactionService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AppliactionService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ApplicationShortInfoDTO?> GetActiveJopAppliactionShortInfo(int RecruiterId)
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
        } //Done

        private async Task<List<ApplicationShortInfoDTO>?> GetApplicationsPaginatedDescending(IQueryable<Application?> BaseQuery,ApplicationsPaginatedRequestDTO requestDTO
            ,int LastApplicationId)
        {
            List<ApplicationShortInfoDTO>? applications = await BaseQuery.Where(A=>A.Id <LastApplicationId)
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
        } //Done

        public async Task<AppliactionsPaginatedDTO> GetApplicationsShortInfo(ApplicationsPaginatedRequestDTO requestDTO,int RecruiterId)
        {
            var BaseQuery = _context.Applications.Where(A =>A.RecruiterId == RecruiterId);

            List<ApplicationShortInfoDTO>? applications = await GetApplicationsPaginatedDescending(BaseQuery,requestDTO,requestDTO.LastApplicationId);

            int RowsCount = 0;
            int LastApplicationId = requestDTO.LastApplicationId;

            if(!requestDTO.IsRowsCountCalculated)
            {
                RowsCount = await BaseQuery.CountAsync();
            }

            if(applications.Count != 0 )
            {
                LastApplicationId=applications[applications.Count-1].ApplicationId;
            }


            return new AppliactionsPaginatedDTO()
            {
                Applications = applications,
                Total = RowsCount,
                LastApplicationId = LastApplicationId,
            };
        }//Done

        public bool IsJopApplicationDataCorrect(AddApplicationDTO addApplicationJopDTO)
        {
            if (string.IsNullOrEmpty(addApplicationJopDTO.Subject) || string.IsNullOrEmpty(addApplicationJopDTO.Description))
                return false;

            if (addApplicationJopDTO.Jop_LevelId <= 0 || addApplicationJopDTO.Jop_TypeId <= 0)
                return false;

            if (addApplicationJopDTO.TimeToComplete == TimeSpan.Zero)
                return false;

            return true;
        } //Done

        public async Task<bool> AddNewJopApplication(AddApplicationDTO addApplicationJopDTO,int RecruiterId,string? AiJobSummary=null)
        {
            var NewApplication = new Application()
            {
                Subject = addApplicationJopDTO.Subject,
                Description = addApplicationJopDTO.Description,
                DateOfCreation = DateTime.Now,
                File_Link = addApplicationJopDTO.File_Link,
                Job_LevelId=addApplicationJopDTO.Jop_LevelId,
                Job_TypeId=addApplicationJopDTO.Jop_TypeId,
                RecruiterId=RecruiterId,
                TimeToComplete=addApplicationJopDTO.TimeToComplete,
                AiSummary=AiJobSummary,
                IsActive=true
            };

            await _context.Applications.AddAsync(NewApplication);
            return await _context.SaveChangesAsync() > 0;
        } //Done

        public async Task<bool> IsJopApplicationForThisRecruiter(int RecruiterId,int JopApplicationId)
        {
            var result = await _context.Applications.Where(J => J.RecruiterId == RecruiterId && J.Id == JopApplicationId).AnyAsync();
            return result;
        } //Done

        public async Task<int> GetNumbersOfAppliedSeekersForTheApplication(int JopApplicationId)
        {
            int SeekersCount = await _context.Applications.Where(J => J.Id == JopApplicationId)
                                                              .Select(J => J.NumbersOfApplied)
                                                              .FirstOrDefaultAsync();

            return SeekersCount;
        } //Done

        private IQueryable<Application> GetFilterQueryForListingJobApplicationsCards(ApplicationsListRequestDTO requestDTO)
        {
            IQueryable<Application> query = _context.Applications.Where(J => J.Id < requestDTO.LastApplicationId && J.IsActive);

            if(requestDTO.JobLevelId !=0 && requestDTO.JobLevelId is not null)
            {
                query=query.Where(J=>J.Job_LevelId==requestDTO.JobLevelId);
            }

            if(requestDTO.JobTypeId != 0 && requestDTO.JobTypeId is not null)
            {
                query=query.Where(J=>J.Job_TypeId==requestDTO.JobTypeId);
            }

            if(requestDTO.CountryId != 0 && requestDTO.CountryId is not null)
            {
                query=query.Where(J=>J.Recruiter.Person.CountryId == requestDTO.CountryId);
            }

            return query;
        }//Done

        public async Task<ApplicationsCardsListResponseDTO> GetJobApplicationsCardsForListing(ApplicationsListRequestDTO requestDTO)
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
                                                                                  CountryName=J.Recruiter.Person.Country.Name,
                                                                                  ShortDescription = J.Description.Length > 200 ? J.Description.Substring(0, 70) : J.Description,
                                                                                  Subject = J.Subject,
                                                                                  DateOfCreation = J.DateOfCreation
                                                                              })
                                                                              .ToListAsync();

            int LastJobApplicationId = requestDTO.LastApplicationId;

            if(applications.Count!=0)
            {
                LastJobApplicationId = applications[applications.Count - 1].ApplicationId;
            }

            return new ApplicationsCardsListResponseDTO()
            {
                applications = applications,
                LastJobApplicationId = LastJobApplicationId
            };
        }//Done

        public async Task<ApplicationFullDetailsDTO?> GetApplicationFullDetailsForSeeker(int jobApplicationId)
        {
            ApplicationFullDetailsDTO? applicationFullDetailsDTO = await _context.Applications.Where(J => J.Id == jobApplicationId && J.IsActive == true)
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
                                                                                                          FileLink = J.File_Link,
                                                                                                          TypeName=J.ApplicationType.Name,
                                                                                                          IsUnviersityApplication=J.UniversityId!=null,
                                                                                                          UniversityId=J.UniversityId
                                                                                                      })
                                                                                                      .AsNoTracking()
                                                                                                      .FirstOrDefaultAsync();

            return applicationFullDetailsDTO;
        } //stay

        public async Task<ApplicationFullDetailsDTO?> GetJobPublicApplicatioDetails(int jobApplicationId)
        {
            ApplicationFullDetailsDTO? applicationFullDetailsDTO = await _context.Applications.Where(J => J.Id == jobApplicationId && J.IsActive == true)
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
                                                                                                          IsUnviersityApplication=J.UniversityId !=null,
                                                                                                          UniversityId = J.UniversityId,
                                                                                                      })
                                                                                                      .AsNoTracking()
                                                                                                      .FirstOrDefaultAsync();

            return applicationFullDetailsDTO;
        } //Done

        public async Task<bool> IsJobApplicationHasMaximumNumber(int jobApplicationId)
        {
            int CurrentNumbersOfApplied = await _context.Applications.Where(J => J.Id == jobApplicationId)
                                                                           .Select(J => J.NumbersOfApplied)
                                                                           .FirstOrDefaultAsync();

            int MaximumSeekersAppliedNumber = Convert.ToInt32(_configuration.GetSection("MaximumSeekersAppliedNumber").Value);

            bool IsHasMaximumNumber = CurrentNumbersOfApplied == MaximumSeekersAppliedNumber;

            return IsHasMaximumNumber;
        } //Done

        public async Task IncreaseNumberOfAppliedByOne(int jobApplicationId)
        {
            await _context.Applications.Where(J=>J.Id==jobApplicationId).ExecuteUpdateAsync(sp => sp.SetProperty(p => p.NumbersOfApplied, p => p.NumbersOfApplied + 1));
        } //Done

    }
}
