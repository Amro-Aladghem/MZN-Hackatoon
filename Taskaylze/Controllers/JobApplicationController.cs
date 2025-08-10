using DTOs.JopAppliaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Coordinators;
//using Services.ExternalServices.AIServices;
//using Services.ExternalServices.AIServices.Interfaces;
using Services.ExternalServices.UploadServices;
using Services.ApplicactionServices;
using Services.JobApplicationResultServices;
using Services.RecruiterServices;
using Services.RecruiterServices.Interfaces;
using System.Security.Claims;
using Services;

namespace Taskalayze.Controllers
{
    [Route("api/v1/jop-applications")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        private readonly AppliactionService _jopAppliactionService;
        private readonly IEnumerable<IUploadService> _uploadServices;
        private readonly IRecruiterApplicationUsageService _recruiterApplicationUsage;
        private readonly ApplicationPostingCoordinatorService _applicationPostingCoordinatorService;
        //private readonly IAiApplicationSummery _applicationSummeryService;
        private readonly JobApplicationResultService _jobApplicationResultService;
        public readonly IApplicationManibulate _applicationManibulate;

        public JobApplicationController(AppliactionService jopAppliactionService, IEnumerable<IUploadService> uploadService,IRecruiterApplicationUsageService recruiterApplicationUsage,
            ApplicationPostingCoordinatorService applicationPostingCoordinatorService,JobApplicationResultService jobApplicationResultService,
            IApplicationManibulate applicationManibulate)
        {
            _jopAppliactionService = jopAppliactionService;
            _uploadServices = uploadService;
            _recruiterApplicationUsage = recruiterApplicationUsage;
            _applicationPostingCoordinatorService = applicationPostingCoordinatorService;
            //_applicationSummeryService = applicationSummeryService;
            _jobApplicationResultService=jobApplicationResultService;
            _applicationManibulate = applicationManibulate;
        }

        [Authorize(Policy = "RequiresRecruiterRole")]
        [HttpGet("active/recent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetMostActiveRecentApplicationForRecruiter()
        {
            var recrutierId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!int.TryParse(recrutierId, out int RecruiterId))
                return BadRequest(new { message = "Invalied recuriter id format!" });

            var application= await _applicationManibulate.GetActiveAppliactionShortInfo(RecruiterId);

            if (application is null)
                return NotFound(new { message = "No Application Was Found " });

            return Ok(new { application }) ;
        } //

        [Authorize(Policy = "RequiresRecruiterRole")]
        [HttpGet("recent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetMostRecentApplictionForRecruiter([FromQuery] ApplicationsPaginatedRequestDTO requestDTO)
        {
            var recrutierId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!int.TryParse(recrutierId, out int RecruiterId))
                return BadRequest(new { message = "Invalied recuriter id format!" });

            var applicationsResult = await _applicationManibulate.GetApplicationsShortInfo(requestDTO, RecruiterId);

            if (applicationsResult.Applications.Count == 0)
                return NotFound(new { message = "No Applications Was Found" });
   

            return Ok(new { applicationsResult });
        } //

        //This Will Not work don't try it 
        //[Authorize(Policy = "RequiresRecruiterRole")]
        //[HttpPost("add")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //public async Task<ActionResult> AddNewApplication([FromForm] AddApplicationDTO addApplicationJopDTO,IFormFile? File)
        //{
        //    var recrutierId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        //    if (!int.TryParse(recrutierId, out int RecruiterId))
        //        return BadRequest(new { message = "Invalied recuriter id format!" });

        //    if (!_applicationManibulate.IsApplicationDataCorrect(addApplicationJopDTO))
        //        return BadRequest(new { message = "Invalied data!" });

        //    if (!await _recruiterApplicationUsage.IsRecruiterHasAvailableApplicationsInAccount(RecruiterId))
        //        return StatusCode(StatusCodes.Status403Forbidden, new { message = "You don't have enough tasks in your account" });

        //    string? AiApplicationSummary = "This is facke description";
        //    if(File is not null)
        //    {
        //        var BlobUploadService = _uploadServices.OfType<BlobStorageUploadService>().FirstOrDefault();

        //        using(var stream = File.OpenReadStream())
        //        {
        //            addApplicationJopDTO.File_Link = await BlobUploadService!.UploadSpecificTypeAsync(stream, File.FileName,File.ContentType);
        //        }

        //        //AiApplicationSummary = await _applicationSummeryService.GetApplicationSummaryFromFile(addApplicationJopDTO.File_Link,File.ContentType);
        //    }

        //    addApplicationJopDTO.ApplicationTypeId = (int)eApplicationTypeEnum.Job_Appliaction;

        //    bool IsDone = await _applicationPostingCoordinatorService.AddNewJopApplication(addApplicationJopDTO, RecruiterId,AiApplicationSummary);

        //    return Ok(new { IsDone });
        //} // 

        
        //This will not work 
        //[Authorize(Policy = "RequiresRecruiterRole")]
        //[HttpPost("university")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult> AddNewApplicationByInstructor([FromForm] AddApplicationDTO addApplicationJopDTO, IFormFile? File)
        //{
        //    var recrutierId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        //    if (!_applicationManibulate.IsApplicationDataCorrect(addApplicationJopDTO))
        //        return BadRequest(new { message = "Invalied data!" });

        //    if (!await _recruiterApplicationUsage.IsRecruiterHasAvailableApplicationsInAccount(recrutierId))
        //        return StatusCode(StatusCodes.Status403Forbidden, new { message = "You don't have enough tasks in your account" });

        //    bool isHasUniversity = Convert.ToBoolean(User.FindFirst("isHasUniversity").Value);

        //    if (!isHasUniversity)
        //        return Unauthorized(new { message = "You don't allow to add university application" });

        //    string? AiApplicationSummary = "This is fack";
        //    if (File is not null)
        //    {
        //        var BlobUploadService = _uploadServices.OfType<BlobStorageUploadService>().FirstOrDefault();

        //        using (var stream = File.OpenReadStream())
        //        {
        //            addApplicationJopDTO.File_Link = await BlobUploadService!.UploadSpecificTypeAsync(stream, File.FileName, File.ContentType);
        //        }

        //        //AiApplicationSummary = await _applicationSummeryService.GetApplicationSummaryFromFile(addApplicationJopDTO.File_Link, File.ContentType);
        //    }

        //    int UniversityId = Convert.ToInt32(User.FindFirst("universityId").Value);

        //    addApplicationJopDTO.ApplicationTypeId = (int)eApplicationTypeEnum.University_Application;
        //    addApplicationJopDTO.IsUniversityApplication = true;
        //    addApplicationJopDTO.UniversityId = UniversityId;

        //    bool IsDone = await _applicationPostingCoordinatorService.AddNewJopApplication(addApplicationJopDTO, recrutierId, AiApplicationSummary);

        //    return Ok(new { IsDone });
        //}

        [Authorize(Policy = "RequiresSeekerRole")]
        [HttpGet("active/list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetJobApplicationsCardsForListing([FromQuery] ApplicationsListRequestDTO requestDTO)
        {
            if (requestDTO.Limit == 0)
                return BadRequest(new { message = "Invalied or empty value for limit!" });

            var SeekerCountryId = User.FindFirst(ClaimTypes.Country).Value;

            if (!int.TryParse(SeekerCountryId, out var CountryId))
                return BadRequest(new { message = "Invalied info in token!" });

            requestDTO.CountryId=CountryId;

            ApplicationsCardsListResponseDTO result = await _applicationManibulate.GetApplicationsCardsForListing(requestDTO);

            return Ok(new { result });
        }

        [HttpGet("{applicationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetJobDetails(int applicationId)
        {
            if (applicationId <= 0)
                return BadRequest(new { message = "Invalied or empty values!" });

            ApplicationFullDetailsDTO? jobApplication = await _applicationManibulate.GetPublicApplicationDetails(applicationId);

            if (jobApplication is null)
                return NotFound(new { message = "No Job application found for this Id!" });

            return Ok(new { jobApplication });
        }

        [Authorize(Policy = "RequiresSeekerRole")]
        [HttpGet("{applicationId}/start")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> StartJobApplication(int applicationId)
        {
            var seekerId =Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (await _applicationManibulate.IsApplicationHasMaximumNumber(applicationId))
                return Ok(new JobApplicationStartResponseDTO
                {
                    Success = false,
                    Message = "You can't join this Job Application because it exeeds the maximum number of applied seeker",
                });

            if (await _jobApplicationResultService.IsSeekerHasJoinedTheApplicationBeforeOrBanned(seekerId,applicationId))
                return Ok(new JobApplicationStartResponseDTO
                {
                    Success = false,
                    Message = "You has Joind this Job Application Before Or You Take Banned , You can't join it again !",
                });

            var jobAppliationDetails = await _jopAppliactionService.GetApplicationFullDetailsForSeeker(applicationId);

            if(jobAppliationDetails!.IsUnviersityApplication)
            {
                if (!_applicationManibulate.IsSeekerHasPermisionToJoin(User.FindFirst("isHasUniversity")!.Value, User.FindFirst("universityId")!.Value
                    , (int)jobAppliationDetails.UniversityId!))
                {
                    return Ok(new JobApplicationStartResponseDTO
                    {
                        Success = false,
                        Message = "You don't have permision to solve this task!",
                    });
                }
            }

            int? resultId = await _jobApplicationResultService.AddNewJopApplicationResultWithPendingStatus(seekerId, applicationId);

            if (resultId is null)
                return BadRequest(new { message = "Faild to add result!" });

            return Ok(new JobApplicationStartResponseDTO
            {
                Success = true,
                Message = "",
                JobApplication=jobAppliationDetails
            });
        }
    }
}
