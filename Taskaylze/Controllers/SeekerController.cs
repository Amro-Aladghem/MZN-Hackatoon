using Database.Entites;
using DTOs.Person;
using DTOs.Seeker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Services.ExternalServices.UploadServices;
using Services.PersonServices;
using Services.SeekerServices;
using System.Security.Claims;
using static Services.PersonServices.PersonService;
using static System.Net.Mime.MediaTypeNames;

namespace Taskalayze.Controllers
{
    [Route("api/v1/seekers")]
    [ApiController]
    public class SeekerController : ControllerBase
    {
        private readonly SeekerService _seekerService;
        private readonly PersonService _personService;
        private readonly TokenService _tokenService;
        private readonly IEnumerable<IUploadService> _uploadServices;

        public  SeekerController(SeekerService seekerService, TokenService tokenService,PersonService personService,IEnumerable<IUploadService>uploadServices)
        {
            _seekerService = seekerService;
            _tokenService = tokenService;
            _personService = personService;
            _uploadServices = uploadServices;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> LoginSeeker([FromBody]PersonLoginDTO personLoginDTO)
        {
            if (string.IsNullOrEmpty(personLoginDTO.Email) || string.IsNullOrEmpty(personLoginDTO.Password))
                return BadRequest(new { message = "Invalied or empty values!" });

            AuthenticatedPersonDTO? person = await _personService.AuthenticatedPerson(personLoginDTO);

            if (person is null)
                return Unauthorized(new { message = "Email Or Password is not correct!" });

            if (person.UserType != "Seeker")
                return Unauthorized(new { message = "This Email does not have Seeker Account, Please Sign Up!" });

            SeekerLoginResponseDTO seeker = await _seekerService.GetSeekerLoginInfo(person);

            string token = _tokenService.CreateToken(seeker.SeekerId, "seeker", seeker.CountryId, IsHasUniversity: seeker.IsHasUniversity, UniversityId: seeker.UniversityId);

            Response.Cookies.Append("AuthToken", token, new CookieOptions()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(3)
            });

            return Ok(new { seeker });
        }

        [HttpPost("outh/google")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AuthenticatWithGoogle([FromBody]PersonGoogleRegister personGoogleRegister)
        {
            if (!_personService.IsRegisterGoogleInfoCorrect(personGoogleRegister))
                return BadRequest(new { message = "Invalied or Empty Values!" });

            bool IsExists = await _personService.IsGoogleIdWithExistPerson(personGoogleRegister.IdToken, PersonService.eUsersType.Seeker);

            SeekerLoginResponseDTO? seeker = await _seekerService.AuthenticatRecruiterWithGoogle(personGoogleRegister, IsExists);

            if (seeker is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to login !" });

            string token = _tokenService.CreateToken(seeker.SeekerId, "seeker", seeker.CountryId);

            Response.Cookies.Append("AuthToken", token, new CookieOptions()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(2)
            });

            return Ok(new { seeker });
        }

        [HttpPost("pre-register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> PreRegisterSeeker([FromBody] PersonLoginDTO personLoginDTO)
        {
            if (string.IsNullOrEmpty(personLoginDTO.Email) || string.IsNullOrEmpty(personLoginDTO.Password))
                return BadRequest(new { message = "Invalied or Empty values!" });

            if (await _personService.IsThisEmailHasActivePerson(personLoginDTO.Email))
                return Conflict(new { message = "This Email has been taken, Please use other email!" });

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RegisterSeeker([FromForm] SeekerRegisterDTO seekerRegisterDTO,IFormFile? Image)
        {
            if (!_seekerService.IsSeekerRegisterValuesCorrect(seekerRegisterDTO))
                return BadRequest(new { message = "Invalied or Empty Values!" });

            var PersonId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!int.TryParse(PersonId, out int personId))
                return BadRequest(new { message = "Invalied Person Id Format" });

            if (Image is not null)
            {
                var imageUploadService = _uploadServices.OfType<CloudinaryImageUploadService>().FirstOrDefault();

                using (var stream = Image.OpenReadStream())
                {
                    seekerRegisterDTO.person.ImageURL = await imageUploadService!.UploadAsync(stream, Image.FileName);
                }
            }

            var seeker = await _seekerService.RegisterSeeker(seekerRegisterDTO,personId);

            string token = _tokenService.CreateToken(seeker.SeekerId, "seeker",seeker.CountryId, IsHasUniversity: seeker.IsHasUniversity, UniversityId: seeker.UniversityId);

            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                SameSite = SameSiteMode.None,
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(2)
            });

            return Ok(new { seeker });
        }

        [Authorize(Policy = "RequiresSeekerRole")]
        [HttpGet("profile/me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> SeekerAccountInfo()
        {
            var seekerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!int.TryParse(seekerId, out int SeekerId))
                return BadRequest(new { message = "Invalied or empty seekerId" });

            SeekerAccountInfoDTO? seeker = await _seekerService.GetSeekerAccountInfo(SeekerId);

            if (seeker is null)
                return NotFound(new { message = "No seeker was found with this Id!" });

            return Ok(new { seeker });
        }

        [Authorize(Policy = "RequiresRecruiterRole")]
        [HttpGet("profile/{seekerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> SeekerProfileInfo(int seekerId)
        {
            var recruiterId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!int.TryParse(recruiterId, out int RecruiterId))
                return BadRequest(new { message = "Invalied or empty seekerId" });

            SeekerProfileInfoDTO? seeker = await _seekerService.GetSeekerProfileInfo(seekerId);

            if (seeker is null)
                return NotFound(new { message = "No seeker was found with this Id!" });

            return Ok(new { seeker });
        }

        [Authorize(Policy = "RequiresSeekerRole")]
        [HttpGet("complete-info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SeekerCompleteInfo()
        {
            var seekerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!int.TryParse(seekerId, out int SeekerId))
                return BadRequest(new { message = "Invalied or empty seekerId" });

            SeekerCompleteInfoDTO seekerCompleteInfo = await _seekerService.GetSeekerCompleteInfo(SeekerId);

            return Ok(new { seekerCompleteInfo });
        }

        [Authorize(Policy = "RequiresSeekerRole")]
        [HttpPut("update/complete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateSeekerCompleteInfo([FromForm]SeekerCompleteInfoDTO seekerCompleteInfoDTO,IFormFile resumeFile)
        {
            var seekerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!int.TryParse(seekerId, out int SeekerId))
                return BadRequest(new { message = "Invalied or empty seekerId" });

            if (!_seekerService.IsCompleteInfoValuesCorrect(seekerCompleteInfoDTO) || resumeFile.Length == 0 || resumeFile is null)
                return BadRequest(new { message = "Invalied or empty values!" });

            var fileUploadeService = _uploadServices.OfType<BlobStorageUploadService>().FirstOrDefault();

            if(!fileUploadeService!.CheckIfFileTypeIsPdf(resumeFile.ContentType,resumeFile.FileName))
                return BadRequest(new { message = "Invalied resume file!" });

            using (var stream =resumeFile.OpenReadStream())
            {
                seekerCompleteInfoDTO.ResumeLink = await fileUploadeService.UploadSpecificTypeAsync(stream, resumeFile.FileName,resumeFile.ContentType);
            }

            bool isDone = await _seekerService.SetSeekerCompleteInfo(seekerCompleteInfoDTO, SeekerId);

            if (!isDone)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "failed to update complete info!" });

            return Ok(new {isDone});
        }

        [Authorize(Policy = "RequiresSeekerRole")]
        [HttpPut("profile/image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult> UpdateProfileImage(IFormFile Image)
        {
            string ImageUrl = string.Empty;

            if (Image is null)
                return BadRequest(new {message= "Invalied or empty image file!" });

            var seekerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if(!int.TryParse(seekerId, out int SeekerId))
                return BadRequest(new { message = "Invalied or empty seekerId" });

            var ImageUploadService = _uploadServices.OfType<CloudinaryImageUploadService>().FirstOrDefault();


            using(var stream = Image.OpenReadStream())
            {
                ImageUrl = await ImageUploadService!.UploadAsync(stream, Image.FileName);
            }

            bool IsDone = await _seekerService.ChangeSeekerProfileImage(SeekerId, ImageUrl);

            if (!IsDone)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to update profile image!" });

            return Ok(new { ImageUrl });
        }

    }

}
