using DTOs.ApplicationResultQuestion;
using DTOs.JobApplicationResult;
using DTOs.JopAppliaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Coordinators;
//using Services.ExternalServices.AIServices.Interfaces;
using Services.ExternalServices.UploadServices;
using Services.ApplicactionServices;
using Services.JobApplicationResultServices;
using System.Security.Claims;

namespace Taskalayze.Controllers
{
    [Route("api/v1/application-results")]
    [ApiController]
    public class JobApplicationResultController : ControllerBase
    {
        private readonly JobApplicationResultService _jobApplicationResultService;
        private readonly AppliactionService _jobAppliactionService;
        private readonly JobApplicationSumbitCoordinatorService _jobApplicationSumbitCoordinatorService;
        private readonly IEnumerable<IUploadService> _uploadServices;
        //private readonly IAiApplicationSumbit _aiApplicationSumbit;
        private readonly JobApplicationResultQuestionsService _jobApplicationResultQuestionsService;

        public JobApplicationResultController(JobApplicationResultService jobApplicationResultService, AppliactionService jobAppliactionService, 
            JobApplicationSumbitCoordinatorService jobApplicationSumbitCoordinatorService, IEnumerable<IUploadService> uploadServices
            , JobApplicationResultQuestionsService jobApplicationResultQuestionsService)
        {
            _jobAppliactionService = jobAppliactionService;
            _jobApplicationResultService = jobApplicationResultService;
            _jobApplicationSumbitCoordinatorService = jobApplicationSumbitCoordinatorService;
            _uploadServices = uploadServices;
            //_aiApplicationSumbit = aiApplicationSumbit;
            _jobApplicationResultQuestionsService = jobApplicationResultQuestionsService;
        }

        [Authorize(Policy = "RequiresRecruiterRole")]
        [HttpGet("{ApplicationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetJobApplicationResults(int ApplicationId, [FromQuery] ApplicationResultsRequestDTO requestDTO)
        {
            if (ApplicationId <= 0)
                return BadRequest(new { message = "Invalied or empty value for application Id!" });

            var recruiterId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!int.TryParse(recruiterId, out int RecruiterId))
                return BadRequest(new { message = "Invalied Recruiter Id Format" });

            if (!await _jobAppliactionService.IsJopApplicationForThisRecruiter(RecruiterId, ApplicationId))
                return Unauthorized(new { message = "you can't show this application result!" });

            ApplicationResultsResponseDTO result = await _jobApplicationResultService.GetJobApplicataionResultsPaginated(requestDTO,ApplicationId);

            if (!requestDTO.IsRowCountCalculated)
            {
                result.NumberOfApplied = await _jobAppliactionService.GetNumbersOfAppliedSeekersForTheApplication(ApplicationId);
            }

            return Ok(new { result });
        }

        [Authorize(Policy ="RequiresSeekerRole")]
        [HttpPut("sumbit/sol-file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> SetSoltionFile([FromQuery] int ApplicationId,IFormFile file)
        {
            var seekerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!int.TryParse(seekerId, out int SeekerId))
                return BadRequest(new { message = "seeker Id format is not correct!" });

            string? fileUri = null;

            var fileUplaoder = _uploadServices.OfType<BlobStorageUploadService>().FirstOrDefault();

            using(var stream = file.OpenReadStream())
            {
                fileUri = await fileUplaoder!.UploadAsync(stream, file.FileName);
            }

            bool isDone = await _jobApplicationResultService.SetSolutionFileUriToApplicationResult(SeekerId,ApplicationId,fileUri);

            return Ok(new { isDone });
        }

        ////This Will Not work don't try it please
        //[Authorize(Policy = "RequiresSeekerRole")]
        //[HttpPut("sumbit")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        //public async Task<ActionResult> SumbitApplicationResult([FromQuery] int ApplicationId)
        //{
        //    int SeekerId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
        //    ApplicationResultSumbitInfoDTO? applciationResultSumbitInfo = await _jobApplicationResultService.GetApplicationResultSumbitInfoForSeekerIfExists(SeekerId, ApplicationId);
            
        //    if (applciationResultSumbitInfo is null)
        //        return Unauthorized(new { message = "you can't sumbit this job application, because you don't have join it before or you have banned!" });

        //    ApplicationResultSumbitResponseDTO response = await _aiApplicationSumbit.GetSumbitAppliction(applciationResultSumbitInfo.ApplicationSummery
        //                                                                                                 , applciationResultSumbitInfo.SolutionFileUri);

        //    TimeSpan? TakenTimeToComplete = _jobApplicationResultService.CalculateTakenTimeToComplete(applciationResultSumbitInfo.DateOfJoined, DateTime.Now,
        //                                                                                              applciationResultSumbitInfo.TimeToCompleteApplication);

        //    if (TakenTimeToComplete is null)
        //        return UnprocessableEntity(new { message = "You have been send the soltion too late!" });

        //    bool isDone = await _jobApplicationSumbitCoordinatorService.HandleApplicationSumbitProcess(response, TakenTimeToComplete.Value, applciationResultSumbitInfo.resultId);

        //    if (!isDone)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Server Error!" });

        //    return Ok(new { response });
        //}

        [Authorize(Policy = "RequiresSeekerRole")]
        [HttpPut("sumbit/test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> SumbitTestQuestions([FromQuery] int ApplicationId, [FromBody]List<ApplicationResultQuestionRequestDTO> questions)
        {
            int SeekerId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            int? resultId = await _jobApplicationResultService.GetApplicationResultIdForSeeker(SeekerId, ApplicationId);

            if (resultId is null)
                return Unauthorized(new { message = "you don't have joind this job application" });

            bool IsPassed = await _jobApplicationResultQuestionsService.IsHasPassedTheTestQuestions(questions, (int)resultId);

            bool IsResultStatusUpdated = await _jobApplicationResultService.SetJobApplicationResultStatusAfterTestExam(IsPassed, (int)resultId);

            await _jobAppliactionService.IncreaseNumberOfAppliedByOne(ApplicationId);

            if (!IsPassed)
                return Ok(new ApplicationResultExamResponseDTO() { IsPassedTheApplication = false, Message = "You Fail In the test,Good luck for next job application task" });

            if (!IsResultStatusUpdated)
                return BadRequest(new { message = "Server Error,please try again!" });

            return Ok(new ApplicationResultExamResponseDTO() { IsPassedTheApplication = true, Message = "Success" });
        }

        [Authorize(Policy = "RequiresSeekerRole")]
        [HttpPut("banned")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> BannedSeekerJobApplicationResult([FromQuery] int ApplicationId)
        {
            int seekerId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (!await _jobApplicationResultService.IsSeekerHasJoinedTheApplicationBeforeOrBanned(seekerId, ApplicationId))
                return Unauthorized(new { message = "You don't have joined this application!" });

            bool IsDone = await _jobApplicationResultService.BannedTheSeekerJobApplicationResult(seekerId, ApplicationId);

            if (!IsDone)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to banned the job application result!" });

            return Ok(new { IsDone });
        }

    }
}
