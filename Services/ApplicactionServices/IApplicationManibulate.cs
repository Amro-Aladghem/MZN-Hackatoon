using DTOs.JopAppliaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApplicactionServices
{
    public interface IApplicationManibulate
    {
        Task IncreaseNumberOfAppliedByOne(int ApplicationId);
        Task<bool> IsApplicationHasMaximumNumber(int ApplicationId);
        Task<ApplicationFullDetailsDTO?> GetPublicApplicationDetails(int ApplicationId);
        Task<ApplicationsCardsListResponseDTO> GetApplicationsCardsForListing(ApplicationsListRequestDTO requestDTO);
        Task<int> GetNumbersOfAppliedSeekersForTheApplication(int ApplicationId);
        Task<bool> IsApplicationForThisRecruiter(int RecruiterId, int JopApplicationId);
        Task<bool> AddNewApplication(AddApplicationDTO addApplicationJopDTO, int RecruiterId, string? AiJobSummary = null);
        Task<AppliactionsPaginatedDTO> GetApplicationsShortInfo(ApplicationsPaginatedRequestDTO requestDTO, int RecruiterId);
        Task<ApplicationShortInfoDTO?> GetActiveAppliactionShortInfo(int RecruiterId);
        bool IsApplicationDataCorrect(AddApplicationDTO addApplicationDTO);
        bool IsSeekerHasPermisionToJoin(string IsHasUniversity,string UniversityId,int ApplicationUniversity);
        
    }
}
