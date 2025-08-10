using Database.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.JobApplicationResultServices;
using DTOs.ApplicationResultQuestion;
using DTOs.JobApplicationResult;

namespace Services.Coordinators
{
    public class JobApplicationSumbitCoordinatorService
    {
        private AppDbContext _context { get; set; }
        private JobApplicationResultService _jobApplicationResultService;
        private JobApplicationResultQuestionsService _jobApplicationResultQuestionsService;

        public JobApplicationSumbitCoordinatorService(AppDbContext context, JobApplicationResultService jobApplicationResultService,
            JobApplicationResultQuestionsService jobApplicationResultQuestionsService)
        {
            _context = context;
            _jobApplicationResultService = jobApplicationResultService;
            _jobApplicationResultQuestionsService = jobApplicationResultQuestionsService;
        }
        public async Task<bool> HandleApplicationSumbitProcess(ApplicationResultSumbitResponseDTO response, TimeSpan TakenTime,int ResultId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                bool IsResultChangedSuccessfully = await _jobApplicationResultService.SetJobApplicationResultUnderCheckStatus(ResultId,TakenTime, response.Percentage);

                if (!IsResultChangedSuccessfully)
                    throw new Exception("Failed to change job application status!");

                bool IsDone = await _jobApplicationResultQuestionsService.AddQuestionForTheJobApplicationResult(ResultId, response.Questions);

                if (!IsDone)
                    throw new Exception("Failed to add questions!");

                await transaction.CommitAsync();

                return true;

            }
        }
     

    }
}
