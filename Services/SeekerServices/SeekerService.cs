using Database.Entites;
using DTOs.Person;
using DTOs.Recruiter;
using DTOs.Seeker;
using Microsoft.EntityFrameworkCore;
using Services.PersonServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Services.PersonServices.PersonService;

namespace Services.SeekerServices
{
    public class SeekerService
    {
        private readonly AppDbContext _context;
        private PersonService _personService;

        public SeekerService(AppDbContext context,PersonService personService)
        {
            _context = context;
            _personService = personService;
        }

        public async Task<SeekerLoginResponseDTO> GetSeekerLoginInfo(AuthenticatedPersonDTO personDTO)
        {
            SeekerLoginResponseDTO? responseDTO = await _context.Seekers.Where(S => S.PersonId == personDTO.PersonId)
                                                                       .Select(S => new SeekerLoginResponseDTO()
                                                                       {
                                                                           SeekerId = S.Id,
                                                                           IsProfileCompleted=S.IsProfileCompleted,
                                                                           IsHasUniversity=S.IsStudent,
                                                                           UniversityId=S.UniversityId
                                                                       })
                                                                       .FirstOrDefaultAsync();

            responseDTO.UserType = "seeker";
            responseDTO.FullName = personDTO.FirstName + " " + personDTO.LastName;
            responseDTO.ImageURL = personDTO.ImageURL;
            responseDTO.CountryId = personDTO.CountryId;

            return responseDTO;
        }

        private async Task<int?> AddNewSeeker(SeekerRegisterDTO seekerRegister,int PersonId)
        {
            var NewSeeker = new Seeker()
            {
                Job_LevelId = seekerRegister.JobLevelId,
                Job_TypeId=seekerRegister.JobTypeId,
                PersonId=PersonId,
                IsProfileCompleted=false,
                LinkedInLink=seekerRegister.LinkeInLink
            };

            if(seekerRegister.person.IsHasUniversity)
            {
                NewSeeker.IsStudent = seekerRegister.person.IsHasUniversity;
                NewSeeker.UniversityId = seekerRegister.person.UniversityId;
                NewSeeker.StudyingYear = seekerRegister.UniversityYear;
                NewSeeker.UniversityStudentNumber = seekerRegister.UniversityNumberId;
            }

            await _context.Seekers.AddAsync(NewSeeker);

            if (await _context.SaveChangesAsync() <= 0)
                return null;

            return NewSeeker.Id;
        }

        public bool IsSeekerRegisterValuesCorrect(SeekerRegisterDTO seekerRegisterDTO)
        {
            if(seekerRegisterDTO.JobLevelId<=0 || seekerRegisterDTO.JobTypeId<=0 || string.IsNullOrEmpty(seekerRegisterDTO.LinkeInLink))
                return false;

            if (!_personService.IsRegisterInfoCorrect(seekerRegisterDTO.person))
                return false;

            return true;
        }

        private async Task<int?> AddNewSeekerWithInitialInfo(int personId)
        {
            var NewSeeker = new Seeker()
            {
                PersonId = personId,
                IsProfileCompleted = false,
            };

            await _context.AddAsync(NewSeeker);

            if (await _context.SaveChangesAsync() <= 0)
                return null;

            return NewSeeker.Id;
        }

        public async Task<SeekerLoginResponseDTO?> RegisterSeeker(SeekerRegisterDTO seekerRegister,int PersonId)
        {
            //remeber to make chack value function 

            PersonRegisterDTO? person = await _personService.RegisterPerson(seekerRegister.person, PersonId,eUsersType.Seeker);

            if (person is null)
                return null;

            int? NewSeekerId = await AddNewSeeker(seekerRegister,PersonId);

            if (NewSeekerId is null)
                return null;

            return new SeekerLoginResponseDTO()
            {
                SeekerId = (int)NewSeekerId,
                IsProfileCompleted = false,
                FullName = person.FirstName + " " + person.LastName,
                ImageURL = person.ImageURL,
                UserType = "seeker",
                CountryId= person.CountryId,
                IsHasUniversity=seekerRegister.person.IsHasUniversity,
                UniversityId=seekerRegister.person.UniversityId
            };
        } //<-------------------------

        private async Task<SeekerLoginResponseDTO> GetSeekerLoginInfoWithGoogleId(string googleId)
        {
            SeekerLoginResponseDTO? responseDTO = await _context.Seekers.Where(R => R.Person.Google_Id == googleId&& R.Person.UserTypeId==(int)eUsersType.Seeker)
                                                                               .Select(R => new SeekerLoginResponseDTO()
                                                                               {
                                                                                   SeekerId = R.Id,
                                                                                   IsProfileCompleted = R.IsProfileCompleted,
                                                                                   FullName = R.Person.FirstName + " " + R.Person.LastName,
                                                                                   ImageURL = R.Person.ImageURL!,
                                                                                   UserType = "seeker",
                                                                                   CountryId= R.Person.CountryId,
                                                                               })
                                                                               .FirstOrDefaultAsync();

            return responseDTO!;
        }

        private async Task<SeekerLoginResponseDTO?> RegisterSeekerWithGoogle(PersonGoogleRegister personGoogleRegister)
        {
            AuthenticatedPersonDTO? personRegisterDTO = await _personService.RegisterPersonByGoogle(personGoogleRegister, eUsersType.Seeker);

            if (personRegisterDTO is null)
                return null;

            int? seekerId = await AddNewSeekerWithInitialInfo((int)personRegisterDTO.PersonId!);

            if (seekerId is null)
                return null;

            return new SeekerLoginResponseDTO()
            {
                SeekerId = (int)seekerId,
                IsProfileCompleted = false,
                FullName = personRegisterDTO.FirstName + " " + personRegisterDTO.LastName,
                ImageURL = personRegisterDTO.ImageURL!,
                UserType = personRegisterDTO.UserType,
            };
        }

        public async Task<SeekerLoginResponseDTO?> AuthenticatRecruiterWithGoogle(PersonGoogleRegister personGoogleRegister, bool IsExists)
        {
            if (IsExists)
            {
                return await GetSeekerLoginInfoWithGoogleId(personGoogleRegister.IdToken);
            }
            else
            {
                return await RegisterSeekerWithGoogle(personGoogleRegister);
            }
        }

        public async Task<SeekerAccountInfoDTO?> GetSeekerAccountInfo(int SeekerId)
        {
            SeekerAccountInfoDTO? seekerInfo = await _context.Seekers.Where(S => S.Id == SeekerId)
                                                                    .Select(S => new SeekerAccountInfoDTO()
                                                                    {
                                                                        SeekerId = S.Id,
                                                                        JobLevelName = S.Job_Level.Name,
                                                                        JobTypeName = S.Job_Type.Name,
                                                                        LinkedInLink = S.LinkedInLink,
                                                                        ResumeLink = S.ResumeLink,
                                                                        IsProfileCompleted = S.IsProfileCompleted,
                                                                        PersonId=S.PersonId
                                                                    })
                                                                    .FirstOrDefaultAsync();

            if (seekerInfo is null)
                return null;

            seekerInfo.Person = await _personService.GetPersonAccountInfoById(seekerInfo.PersonId);

            if (seekerInfo.Person is null)
                return null;

            return seekerInfo;
        }

        public async Task<SeekerProfileInfoDTO?> GetSeekerProfileInfo(int SeekerId)
        {
            SeekerProfileInfoDTO? seekerInfo = await _context.Seekers.Where(S => S.Id == SeekerId)
                                                                .Select(S => new SeekerProfileInfoDTO()
                                                                {
                                                                    SeekerId = S.Id,
                                                                    JobLevelName = S.Job_Level.Name,
                                                                    JobTypeName = S.Job_Type.Name,
                                                                    LinkedInLink = S.LinkedInLink,
                                                                    ResumeLink = S.ResumeLink,
                                                                    PersonId = S.PersonId
                                                                })
                                                                .FirstOrDefaultAsync();

            if (seekerInfo is null)
                return null;


            seekerInfo.Person = await _personService.GetPersonProfileInfoById(seekerInfo.PersonId);

            if (seekerInfo.Person is null)
                return null;

            return seekerInfo;

        }

        public bool IsCompleteInfoValuesCorrect(SeekerCompleteInfoDTO seekerCompleteInfoDTO)
        {
            if(seekerCompleteInfoDTO.CountryId<=0 || seekerCompleteInfoDTO.JobLevelId<=0 || seekerCompleteInfoDTO.JobTypeId<=0 ||
                string.IsNullOrEmpty(seekerCompleteInfoDTO.Phone) || string.IsNullOrEmpty(seekerCompleteInfoDTO.LinkedInLink))
            {
                return false;
            }

            if(seekerCompleteInfoDTO.IsStudent)
            {
                if (seekerCompleteInfoDTO.UniversityId <= 0 || string.IsNullOrEmpty(seekerCompleteInfoDTO.UniversityNumberId))
                    return false;
            }

            return true;
        }

        public async Task<SeekerCompleteInfoDTO> GetSeekerCompleteInfo(int SeekerId)
        {
            SeekerCompleteInfoDTO? info = await _context.Seekers.Where(s => s.Id == SeekerId)
                                                                 .Select(s => new SeekerCompleteInfoDTO()
                                                                 {
                                                                     JobLevelId = s.Job_LevelId == null ? 0 : (int)s.Job_LevelId,
                                                                     JobTypeId = s.Job_TypeId == null ? 0 : (int)s.Job_LevelId,
                                                                     LinkedInLink = s.LinkedInLink,
                                                                     CountryId = s.Person.CountryId == null ? 0 : (int)s.Person.CountryId,
                                                                     Phone = s.Person.Phone
                                                                 })
                                                                 .FirstOrDefaultAsync();

            return info;
        }

        public async Task<bool> SetSeekerCompleteInfo(SeekerCompleteInfoDTO seekerCompleteInfo,int SeekerId)
        {
            Seeker seeker = await _context.Seekers.Where(s => s.Id == SeekerId).SingleAsync();

            seeker.ResumeLink = seekerCompleteInfo.ResumeLink;
            seeker.LinkedInLink = seekerCompleteInfo.LinkedInLink;
            seeker.Job_LevelId = seekerCompleteInfo.JobLevelId;
            seeker.Job_TypeId = seekerCompleteInfo.JobTypeId;
            seeker.IsProfileCompleted = true;

            if(seeker.IsStudent)
            {
                seeker.IsStudent = true;
                seeker.UniversityId = seekerCompleteInfo.UniversityId;
                seeker.UniversityStudentNumber = seekerCompleteInfo.UniversityNumberId;
                seeker.StudyingYear = seekerCompleteInfo.UniversityYear;
            }

            if(await _context.SaveChangesAsync()<=0)
                return false;

            return await _personService.SetPersonCompleteInfoForSeekerType(seekerCompleteInfo, seeker.PersonId);
        }

        public async Task<bool> ChangeSeekerProfileImage(int SeekerId,string ImageUrl)
        {
            int PersonId = await _context.Seekers.Where(s => s.Id == SeekerId)
                                                                .Select(s => s.PersonId)
                                                                .FirstOrDefaultAsync();

            if(PersonId <= 0)
                return false;

            if (!await _personService.ChangeImage(PersonId,ImageUrl))
            {
                return false;
            }

            return true;
        }
    }
}
