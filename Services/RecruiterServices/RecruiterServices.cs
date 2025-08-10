using Database.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.PersonServices;
using DTOs.Recruiter;
using DTOs.Person;
using Microsoft.EntityFrameworkCore;
using static Services.PersonServices.PersonService;
using Microsoft.IdentityModel.Tokens;


namespace Services.RecruiterServices
{
    public class RecruiterService
    {
        private readonly AppDbContext _context;
        private readonly PersonService _personServices;

        public RecruiterService(AppDbContext context, PersonService personServices)
        {
            _context = context;
            _personServices=personServices;
        }

        public async Task<RecruiterLoginResponseDTO> GetRecruiterLoginInfo(AuthenticatedPersonDTO authenticatedPerson)
        {
            RecruiterLoginResponseDTO? recruiter =await  _context.Recruiters.Where(R=>R.PersonId==authenticatedPerson.PersonId)
                                                                             .Select(R=>new RecruiterLoginResponseDTO()
                                                                             {
                                                                                 RecruiterId=R.Id,
                                                                                 IsProfileCompleted=R.IsProfileComplete,
                                                                                 IsHasUniversity=R.IsInstructor,
                                                                                 UniversityId=R.UniversityId
                                                                             })
                                                                            .FirstOrDefaultAsync();

            recruiter!.FullName = authenticatedPerson.FirstName + " " + authenticatedPerson.LastName;
            recruiter.ImageURL = authenticatedPerson.ImageURL;
            recruiter.UserType = authenticatedPerson.UserType;

            return recruiter;
        }

        private async Task<int?> AddNewRecruiterWithInitialInfo(int PersonId,PersonRegisterDTO? personExtraData=null)
        {
            var NewRecruiter = new Recruiter()
            {
                PersonId=PersonId,
                IsProfileComplete = false
            };

            if(personExtraData is not null)
            {
                if(personExtraData.IsHasUniversity)
                {
                    NewRecruiter.IsInstructor = personExtraData.IsHasUniversity;
                    NewRecruiter.UniversityId = personExtraData.UniversityId;
                }
            }

            _context.Recruiters.Add(NewRecruiter);

            if (await _context.SaveChangesAsync() <= 0)
                return null;

            return NewRecruiter.Id;
        }

        public async Task<RecruiterLoginResponseDTO?> RegisterRecruiter(PersonRegisterDTO personRegister,int PersonId)
        {
            PersonRegisterDTO? personRegisterDTO = await _personServices.RegisterPerson(personRegister,PersonId,eUsersType.Recruiter);

            if (personRegisterDTO is null)
                return null;
                
            int? recruiterId = await  AddNewRecruiterWithInitialInfo(PersonId, personRegister);

            if (recruiterId is null)
                return null;

            return new RecruiterLoginResponseDTO()
            {
                RecruiterId = (int)recruiterId,
                IsProfileCompleted = false,
                FullName = personRegisterDTO.FirstName + " " + personRegisterDTO.LastName,
                ImageURL = personRegisterDTO.ImageURL!,
                UserType = personRegisterDTO.UserType,
                IsHasUniversity=personRegister.IsHasUniversity,
                UniversityId=personRegister.UniversityId
            };
        }

        private async Task<RecruiterLoginResponseDTO?> RegisterRecruiterWithGoogle(PersonGoogleRegister personGoogleRegister)
        {
            AuthenticatedPersonDTO? personRegisterDTO = await _personServices.RegisterPersonByGoogle(personGoogleRegister, eUsersType.Recruiter);

            if (personRegisterDTO is null)
                return null;

            int? recruiterId = await AddNewRecruiterWithInitialInfo((int)personRegisterDTO.PersonId!);

            if (recruiterId is null)
                return null;

            return new RecruiterLoginResponseDTO()
            {
                RecruiterId = (int)recruiterId,
                IsProfileCompleted = false,
                FullName = personRegisterDTO.FirstName + " " + personRegisterDTO.LastName,
                ImageURL = personRegisterDTO.ImageURL!,
                UserType = personRegisterDTO.UserType,
            };
        }

        private async Task<RecruiterLoginResponseDTO> GetRecruiterLoginInfoWithGoogleId(string googleId)
        {
            RecruiterLoginResponseDTO? responseDTO = await _context.Recruiters.Where(R => R.Person.Google_Id == googleId && R.Person.UserTypeId==(int)eUsersType.Recruiter)
                                                                               .Select(R => new RecruiterLoginResponseDTO()
                                                                               {
                                                                                   RecruiterId = R.Id,
                                                                                   IsProfileCompleted = R.IsProfileComplete,
                                                                                   FullName = R.Person.FirstName + " " + R.Person.LastName,
                                                                                   ImageURL = R.Person.ImageURL!,
                                                                                   UserType = "recruiter"
                                                                               })
                                                                               .FirstOrDefaultAsync();

            return responseDTO!;
        }

        public async Task<RecruiterLoginResponseDTO?> AuthenticatRecruiterWithGoogle(PersonGoogleRegister personGoogleRegister,bool IsExists)
        {
            if(IsExists)
            {
                return await GetRecruiterLoginInfoWithGoogleId(personGoogleRegister.IdToken);
            }
            else
            {
                return await RegisterRecruiterWithGoogle(personGoogleRegister);
            }
        }

        public async Task<RecruiterAccountInfoDTO?> GetRecruiterAccountInfo(int RecruiterId)
        {
            RecruiterAccountInfoDTO? recruiter = await _context.Recruiters.Where(R => R.Id == RecruiterId)
                                                                             .Select(R => new RecruiterAccountInfoDTO()
                                                                             {
                                                                                 RecruiterId = R.Id,
                                                                                 CompanyName = R.Company.Name,
                                                                                 IsFreelancer = R.IsFreelancer,
                                                                                 IsProfileComplete = R.IsProfileComplete,
                                                                                 LinkedIn = R.Linked_In_Link,
                                                                                 PersonId = R.PersonId,
                                                                             })
                                                                             .FirstOrDefaultAsync();

            if (recruiter is null) return null;

            recruiter.Person = await _personServices.GetPersonAccountInfoById(recruiter.PersonId);

            if (recruiter.Person is null)
                return null;

            return recruiter;
        }

        public async Task<bool> SetCompleteProfileInfo(RecruiterCompleteProfileDTO completeProfileDTO,int RecruiterId,int? CompanyId)
        {
            int UpdatedNumRows = await _context.Recruiters.Where(R => R.Id == RecruiterId).ExecuteUpdateAsync(s =>
                                                                                          s.SetProperty(s => s.CompanyId, CompanyId)
                                                                                          .SetProperty(s => s.IsFreelancer, completeProfileDTO.IsFreelancer)
                                                                                          .SetProperty(s=>s.Linked_In_Link,completeProfileDTO.LinkedInLink)
                                                                                          .SetProperty(s=>s.IsProfileComplete,true)
                                                                                          );

            return UpdatedNumRows > 0;

        }

        public async Task<bool> IsRecruiterPrfoileCompleted(int RecruiterId)
        {
            bool IsProfileCompleted = await _context.Recruiters.Where(R => R.Id == RecruiterId && R.IsProfileComplete==true).AnyAsync();

            return IsProfileCompleted;
        }

        public async Task<(bool IsInstructor,int?UniversityId)> CheckIfRecruiterIsInstructor(int RecruiterId)
        {
            (bool IsInstructor, int? UniversityId) checkResult = await _context.Recruiters.Where(R => R.Id == RecruiterId)
                                                                                          .Select(R => new ValueTuple<bool, int?>(R.IsInstructor, R.UniversityId))
                                                                                          .FirstOrDefaultAsync();
            
            return checkResult;
        }
    }
}
