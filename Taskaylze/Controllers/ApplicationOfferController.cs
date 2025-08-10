using DTOs.JopApplicationOffer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.ExternalServices;
using Services.JobApplicationOfferService;
using System.Security.Claims;

namespace Taskalayze.Controllers
{
    [Route("api/v1/offers")]
    [ApiController]
    public class ApplicationOfferController : ControllerBase
    {
        private readonly JobAppliactionOfferService _jopApplicationOfferServices;
        private readonly WhatsAppLinkService _whatsAppLinkService;

        public ApplicationOfferController(JobAppliactionOfferService jopAppliactionOfferService,WhatsAppLinkService whatsAppLinkService)
        {
            _jopApplicationOfferServices= jopAppliactionOfferService;
            _whatsAppLinkService = whatsAppLinkService;
        }

        [Authorize(Policy = "RequiresRecruiterRole")]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAvaiableOffers()
        
        {
            List<ApplicationOfferDTO>? offers = await _jopApplicationOfferServices.GetJopApplicationOffers();

            return Ok(new { offers });
        }

        [Authorize(Policy = "RequiresRecruiterRole")]
        [HttpGet("{offerId}/buy/pay-choice/contact")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> BuyOfferWithContactOffer(int offerId)
        {
            var recriterId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!int.TryParse(recriterId, out int RecruiterId))
                return BadRequest(new { message = "Invalied or empty recuirter Id" });

            string? OfferTitle = await _jopApplicationOfferServices.GetOfferTitleIfExistsById(offerId);

            if (OfferTitle is null)
                return BadRequest(new { message = "No offer with this id!" });

            string message = $"""
                مرحبا أريد شراء عرض ال Tasks 
                رقم معرفي : ${RecruiterId}
                رقم العرض : ${offerId}
                اسم العرض : {OfferTitle}
                """;

            string link = _whatsAppLinkService.GetContanctWhatsAppLink(message);

            return Ok(new { link });
        }


    }
}
