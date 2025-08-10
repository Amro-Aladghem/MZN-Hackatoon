using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services;
using Services.PersonServices;
using Services.RecruiterServices;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using DTOs.Person;
using Microsoft.AspNetCore.Authorization;
using static Services.PersonServices.PersonService;
using Database.Entites;
using Microsoft.OpenApi.Writers;
using Services.ExternalServices.UploadServices;
using Services.RecruiterServices.Interfaces;
using Services.Coordinators;
using DTOs.Recruiter;

namespace Taskalayze.Controllers
{
    [Route("api/v1/recruiters")]
    [ApiController]
    public class RecruiterController : ControllerBase
    {
        private readonly RecruiterService _recruiterService;
        private readonly PersonService _personService;
        private readonly TokenService _tokenService;
        private readonly IEnumerable<IUploadService> _uploadServices;
        private readonly IRecruiterApplicationUsageService _recruiterApplicationUsage;
        private readonly RecruiterCompanyCoordinatorService _recruiterCompanyCoordinatorService;
        private readonly RecruiterLaunshCoordinatorService _recruiterLaunshCoordinatorService;

        public RecruiterController(RecruiterService recruiterService, PersonService personService,TokenService tokenService,IEnumerable<IUploadService> uploadServices,
            IRecruiterApplicationUsageService recruiterApplicationUsage, RecruiterCompanyCoordinatorService recruiterCompanyCoordinatorService, RecruiterLaunshCoordinatorService recruiterLaunshCoordinatorService)
        {
            _recruiterService = recruiterService;
            _personService = personService;
            _tokenService = tokenService;
            _uploadServices = uploadServices;
            _recruiterApplicationUsage = recruiterApplicationUsage;
            _recruiterCompanyCoordinatorService = recruiterCompanyCoordinatorService;
            _recruiterLaunshCoordinatorService = recruiterLaunshCoordinatorService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Login([FromBody]PersonLoginDTO personLoginDTO)
        {
            if (string.IsNullOrEmpty(personLoginDTO.Password) || string.IsNullOrEmpty(personLoginDTO.Email))
                return BadRequest(new { message = "Invalied or Empty values!" });

           
            var authenticatedPerson = await _personService.AuthenticatedPerson(personLoginDTO);

            if (authenticatedPerson is null)
                return Unauthorized(new { message = "Email Or Password is not correct!" });

            if(authenticatedPerson.UserType!="Recruiter")
                return Unauthorized(new { message = "This Email does not has a Recruiter Account,Please Sign Up" });

            var recruiter = await _recruiterService.GetRecruiterLoginInfo(authenticatedPerson);
            
            string token = _tokenService.CreateToken(recruiter.RecruiterId, "recruiter",IsHasUniversity:recruiter.IsHasUniversity,UniversityId:recruiter.UniversityId);

            Response.Cookies.Append("AuthToken", token, new CookieOptions()
            {
                SameSite = SameSiteMode.None,
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(2)
            });

            return Ok(new { recruiter });
        }

        [HttpPost("outh/google")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AuthWithGoogle([FromForm]PersonGoogleRegister personGoogleRegister)
        {
            if (!_personService.IsRegisterGoogleInfoCorrect(personGoogleRegister))
                return BadRequest(new { message = "Invalied or Empty Values!" });

            bool IsRecruiterExists = await _personService.IsGoogleIdWithExistPerson(personGoogleRegister.IdToken, eUsersType.Recruiter);

            var recruiter = await _recruiterService.AuthenticatRecruiterWithGoogle(personGoogleRegister, IsRecruiterExists);

            if (recruiter is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Faild to login, please try again !" });

            string token = _tokenService.CreateToken(recruiter.RecruiterId, "recruiter");

            Response.Cookies.Append("AuthToken", token, new CookieOptions()
            {
                SameSite = SameSiteMode.None,
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(2)
            });

            return Ok(new { recruiter });
        }


        [HttpPost("pre-register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PreRegister(PersonLoginDTO personLoginDTO)
        {
            if (string.IsNullOrEmpty(personLoginDTO.Password) || string.IsNullOrEmpty(personLoginDTO.Email))
                return BadRequest(new { message = "Invalied or Empty values!" });

            if (await _personService.IsThisEmailHasActivePerson(personLoginDTO.Email))
                return Conflict(new { message = "This Email has been taken , Use other email!" });

            var PreRegisterResult = await _personService.PreRegiseterPerson(personLoginDTO, eUsersType.Person);

            string token = _tokenService.CreateToken(PreRegisterResult.PersonId, "person");

            Response.Cookies.Append("AuthToken", token, new CookieOptions()
            {
                SameSite = SameSiteMode.None,
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return Ok(new { PreRegisterResult });
        }

        [Authorize(Policy = "RequiresPersonRole")]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RegisterPerson([FromForm]PersonRegisterDTO personRegisterDTO,IFormFile? Image)
        {
            if (!_personService.IsRegisterInfoCorrect(personRegisterDTO))
                return BadRequest(new { message = "Invalied Or Empty values!" });

            var PersonId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            
            if (!int.TryParse(PersonId, out int personId))
                return BadRequest(new { message = "Invalied Person Id Format" });

            if(Image is not null)
            {
                var imageUploadService = _uploadServices.OfType<CloudinaryImageUploadService>().FirstOrDefault();

                using(var stream =Image.OpenReadStream())
                {
                    personRegisterDTO.ImageURL = await imageUploadService.UploadAsync(stream, Image.FileName);
                }
            }

            var recruiter = await _recruiterLaunshCoordinatorService.LaunshRecruiterAccount(personRegisterDTO, personId);

            if (recruiter is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to register account,Please try again" });

            string token = _tokenService.CreateToken(recruiter.RecruiterId, "recruiter",IsHasUniversity: recruiter.IsHasUniversity, UniversityId: recruiter.UniversityId);

            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                SameSite = SameSiteMode.None,
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(2)
            });

            return Ok(new { recruiter });
        }

        [Authorize(Policy = "RequiresRecruiterRole")]
        [HttpGet("uasage/available")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetNumberOfAvailableApplicationsInAccount()
        {
            var recrutierId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!int.TryParse(recrutierId, out int RecruiterId))
                return BadRequest(new { message = "Invalied recuriter id format!" });

            int Available = await _recruiterApplicationUsage.GetNumberOfAvailableTaskForRecruiter(RecruiterId);

            return Ok(new { Available });
        }

        [Authorize(Policy = "RequiresRecruiterRole")]
        [HttpGet("profile/me")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetRecruiterAccountInfo()
        {
            var recrutierId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!int.TryParse(recrutierId, out int RecruiterId))
                return BadRequest(new { message = "Invalied recuriter id format!" });

            var recruiter = await _recruiterService.GetRecruiterAccountInfo(RecruiterId);

            if (recruiter is null)
                return NotFound(new { message = "No Recruiter was found!" });

            return Ok(new { recruiter });
        }

        [Authorize(Policy = "RequiresRecruiterRole")]
        [HttpPut("profile/me/complete")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> SetCompleteRecruiterInfo([FromForm] RecruiterCompleteProfileDTO completeProfileDTO)
        {
            var recrutierId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!int.TryParse(recrutierId, out int RecruiterId))
                return BadRequest(new { message = "Invalied recuriter id format!" });

            if (await _recruiterService.IsRecruiterPrfoileCompleted(RecruiterId))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You are already completed your profile!" });

            bool IsDone = await _recruiterCompanyCoordinatorService.HandleCompleteProfileInfoProcess(completeProfileDTO, RecruiterId);

            return Ok(new { IsDone });
        }


    }
}
