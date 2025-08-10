using Database.Entites;
using DTOs.JobApplicationResult;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Services.JobApplicationResultServices
{
    public class JobApplicationResultService
    {
        public enum eResultStatus { Accepted=1,Under_Checked=2,Not_Accepted=3,Pending=4,banned=5};

        private readonly AppDbContext _context;

        public JobApplicationResultService(AppDbContext context)
        {
            _context = context;
        }

        private async Task<List<ApplicationResultListDTO>?> GetApplicationResultsForListing(IQueryable<Application_Result> query, ApplicationResultsRequestDTO requestDTO)
        {
            List<ApplicationResultListDTO>? results = await query.OrderByDescending(J => J.Result)
                                                                .OrderBy(J => J.TakenTimeToComplete)
                                                                .Skip((requestDTO.Page-1)*requestDTO.Limit)
                                                                .Take(requestDTO.Limit)
                                                                .Select(J => new ApplicationResultListDTO()
                                                                {
                                                                    ApplicationResultId = J.Id,
                                                                    SeekerId = J.SeekerId,
                                                                    TimeToComplete = $"{J.TakenTimeToComplete.Value.Hours:D2}:{J.TakenTimeToComplete.Value.Minutes:D2}",
                                                                    ResultPercentage = (decimal)J.Result!,
                                                                    SeekerName = J.Seeker.Person.FirstName + " " + J.Seeker.Person.LastName,
                                                                    SeekerImage = J.Seeker.Person.ImageURL!,
                                                                    DateOfResult = J.DateAndTimeOfJoined,
                                                                    SolutionFile= J.SolutionFileUri
                                                                })
                                                                .ToListAsync();
            return results;
        }

        public async Task<ApplicationResultsResponseDTO> GetJobApplicataionResultsPaginated(ApplicationResultsRequestDTO requestDTO, int JobApplicationId)
        {
            int acceptedStatus = (int)eResultStatus.Accepted;

            var BaseQuery = _context.Application_Results.Where(J => J.ApplicationId == JobApplicationId && J.Application_Result_StatusId == acceptedStatus);

            List<ApplicationResultListDTO>? ApplicationResults = await GetApplicationResultsForListing(BaseQuery, requestDTO);

            int TotalCount = 0;

            if(!requestDTO.IsRowCountCalculated)
            {
                TotalCount = await BaseQuery.CountAsync();
            }

            return new ApplicationResultsResponseDTO()
            {
                ApplicationResults = ApplicationResults,
                NextPage = requestDTO.Page + 1,
                TotalCount=TotalCount
            };
        }

        public async Task<bool> IsSeekerHasJoinedTheApplicationBeforeOrBanned(int SeekerId,int ApplicationId)
        {
            var result = await _context.Application_Results.Where(JR => JR.SeekerId == SeekerId && JR.ApplicationId == ApplicationId)
                                                               .AsNoTracking()
                                                               .FirstOrDefaultAsync();
            if (result is null)
                return false;

            if (result.Application_Result_StatusId==(int)eResultStatus.banned)
                return true ;

            return true;
        }

        public async Task<int?> AddNewJopApplicationResultWithPendingStatus(int SeekerId,int ApplicationId)
        {
            var newJobResult = new Application_Result()
            {
                SeekerId = SeekerId,
                ApplicationId = ApplicationId,
                Application_Result_StatusId = (int)eResultStatus.Pending
            };

            await _context.Application_Results.AddAsync(newJobResult);

            if (await _context.SaveChangesAsync() <= 0)
                return null;

            return newJobResult.Id;
        }

        public async Task<ApplicationResultSumbitInfoDTO?> GetApplicationResultSumbitInfoForSeekerIfExists(int SeekerId,int ApplicationId)
        {
            var applicationResultInfo = await _context.Application_Results.Where(JR => JR.ApplicationId == ApplicationId
                                                                                         && JR.SeekerId == SeekerId
                                                                                         && JR.SolutionFileUri != null
                                                                                         && JR.Application_Result_StatusId==(int)eResultStatus.Pending)
                                                                                        .Select(JR => new ApplicationResultSumbitInfoDTO()
                                                                                        {
                                                                                            resultId = JR.Id,
                                                                                            DateOfJoined = JR.DateAndTimeOfJoined,
                                                                                            SolutionFileUri = JR.SolutionFileUri!,
                                                                                            ApplicationSummery=JR.Application.AiSummary,
                                                                                            TimeToCompleteApplication=JR.Application.TimeToComplete
                                                                                        })
                                                                                         .FirstOrDefaultAsync();

            return applicationResultInfo;
        }

        public async Task<bool> IsSeekerJoinedTheApplication(int SeekerId,int ApplicationId)
        {
            bool IsExists = await _context.Application_Results.Where(JR=>JR.SeekerId==SeekerId && JR.ApplicationId==ApplicationId).AnyAsync();

            return IsExists;
        }

        public async Task<bool> SetJobApplicationResultUnderCheckStatus(int resultId,TimeSpan TakenTimeToComplete,int Percentage)
        {
            int NumberOfUpdatedRows = await _context.Application_Results
                                                    .Where(R=>R.Id==resultId)
                                                    .ExecuteUpdateAsync(sp => sp.SetProperty(p => p.TakenTimeToComplete, TakenTimeToComplete)
                                                    .SetProperty(p => p.Application_Result_StatusId, (int)eResultStatus.Under_Checked)
                                                    .SetProperty(p => p.Result, Percentage));

            return NumberOfUpdatedRows != 0;
        }

        public TimeSpan? CalculateTakenTimeToComplete(DateTime DateOfJoined, DateTime DateOfSumbit,TimeSpan ApplicationTimeToComplete)
        {
            if (DateOfSumbit.Date != DateOfJoined.Date)
                return null;


            TimeSpan TakenTime = DateOfSumbit - DateOfJoined;

            if (TakenTime > ApplicationTimeToComplete.Add(TimeSpan.FromSeconds(3)))
                return null;

            return TakenTime;

        }

        public async Task<bool> SetSolutionFileUriToApplicationResult(int SeekerId,int ApplicationId,string SolutionFileUri)
        {
            int NumberOfUpdatedRows = await _context.Application_Results.Where(JR => JR.SeekerId == SeekerId && JR.ApplicationId == ApplicationId)
                                                                             .ExecuteUpdateAsync(sp => sp.SetProperty(p => p.SolutionFileUri, SolutionFileUri));

            return NumberOfUpdatedRows != 0;
        }

        public async Task<bool> SetJobApplicationResultStatusAfterTestExam(bool IsPassed,int resultId)
        {
            int ResultStatusId = IsPassed ? (int)eResultStatus.Accepted : (int)eResultStatus.Not_Accepted;

            int NumberOfRowsAffected = await _context.Application_Results.Where(JR => JR.Id == resultId)
                                                                             .ExecuteUpdateAsync(sp => sp.SetProperty(p => p.Application_Result_StatusId, ResultStatusId));

            return NumberOfRowsAffected != 0;
        }

        public async Task<int?> GetApplicationResultIdForSeeker(int SeekerId,int ApplicationId)
        {
            int? resultId = await _context.Application_Results.Where(JR => JR.SeekerId == SeekerId && JR.ApplicationId == ApplicationId
                                                                          && JR.Application_Result_StatusId == (int)eResultStatus.Under_Checked)
                                                                          .Select(j=>j.Id)
                                                                         .FirstOrDefaultAsync();

            return resultId;
        }

        public async Task<bool> BannedTheSeekerJobApplicationResult(int SeekerId,int ApplicationId)
        {
            int NumberOfUpdatedRows = await _context.Application_Results.Where(JR => JR.SeekerId == SeekerId && JR.ApplicationId == ApplicationId)
                                                                             .ExecuteUpdateAsync(sp => sp.SetProperty(p => p.Application_Result_StatusId, (int)eResultStatus.banned));

            return NumberOfUpdatedRows > 0;
        }

    }
}
