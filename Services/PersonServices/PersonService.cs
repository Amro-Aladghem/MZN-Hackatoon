using Azure.Identity;
using Database.Entites;
using DTOs.Person;
using DTOs.Seeker;
using Microsoft.EntityFrameworkCore;
using Services.SecurityServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.PersonServices
{
    public class PersonService
    {
        public enum eUsersType {Recruiter=2,Seeker=1,Person=3};

        private readonly AppDbContext _context;
        private readonly ISecurityService _securityService;
        private const string DefaultImageURL = "https://res.cloudinary.com/dlu3aolnh/image/upload/v1750941990/user-square_rhmoxo.png";
        public PersonService(AppDbContext context,ISecurityService securityService)
        {
            _context = context;
            _securityService= securityService;
        }

        public bool IsRegisterInfoCorrect(PersonRegisterDTO personRegisterDTO)
        {
            if (string.IsNullOrEmpty(personRegisterDTO.FirstName) || string.IsNullOrEmpty(personRegisterDTO.LastName) 
                      || string.IsNullOrEmpty(personRegisterDTO.UserType) || string.IsNullOrEmpty(personRegisterDTO.PhoneNumber))
                return false;

            return true;
        }

        public bool IsRegisterGoogleInfoCorrect(PersonGoogleRegister personGoogleRegister)
        {
            if(string.IsNullOrEmpty(personGoogleRegister.FirstName)||string.IsNullOrEmpty(personGoogleRegister.LastName)||
                string.IsNullOrEmpty(personGoogleRegister.IdToken)||string.IsNullOrEmpty(personGoogleRegister.Email) )
            {
                return false;
            }

            return true;
        }

        public async Task<bool>IsGoogleIdWithExistPerson(string googleId,eUsersType userType)
        {
            int userTypeId = (int)userType;

            bool IsExists = await _context.Persons.Where(P => P.Google_Id == googleId && P.UserTypeId==userTypeId).AnyAsync();

            return IsExists;
        }

        public async Task<AuthenticatedPersonDTO?> AuthenticatedPerson(PersonLoginDTO personLoginDTO)
        {
            var person  = await _context.Persons.Where(P => P.Email == personLoginDTO.Email && P.IsActive==true)
                                                .Select(P => 
                                                new {P.Id,P.Password,P.ImageURL,P.Email,P.FirstName,P.LastName,P.UserTypeId,P.CountryId})
                                                .FirstOrDefaultAsync();

            if (person is null)
                return null;

            if (!_securityService.VerifyEncrypt(person.Password!, personLoginDTO.Password))
                return null;

            string UserType = ((eUsersType)person.UserTypeId).ToString();

            return new AuthenticatedPersonDTO
            {
                PersonId = person.Id,
                Email = personLoginDTO.Email,
                FirstName = person.FirstName,
                LastName = person.LastName,
                UserType = UserType,
                CountryId=person.CountryId,
                ImageURL=person.ImageURL
            };
        }

        public async Task<AuthenticatedPersonDTO?> AuthenticatedPersonWithGoogleId(PersonGoogleLoginDto personGoogleLoginDto)
        {
            var person = await _context.Persons.Where(P => P.Email == personGoogleLoginDto.Email && P.IsActive==true)
                                               .Select(P => new {P.Id,P.Google_Id,P.ImageURL,P.UserTypeId,P.FirstName,P.LastName})
                                               .FirstOrDefaultAsync();

            if (person is null)
                return null;

            if(person.Google_Id != personGoogleLoginDto.IdToken) return null;

            string UserType = ((eUsersType)person.UserTypeId).ToString();

            return new AuthenticatedPersonDTO()
            {
                PersonId = person.Id,
                Email = personGoogleLoginDto.Email,
                FirstName = person.FirstName!,
                LastName = person.LastName!,
                ImageURL = person.ImageURL!,
                UserType = UserType
            };
        }   //<--- it is not used!

        public async Task<AuthenticatedPersonDTO?> RegisterPersonByGoogle(PersonGoogleRegister personGoogleRegister,eUsersType eUser)
        {
            var NewPerson = new Person()
            {
                Email = personGoogleRegister.Email,
                Google_Id = personGoogleRegister.IdToken,
                FirstName = personGoogleRegister.FirstName,
                LastName = personGoogleRegister.LastName,
                ImageURL = personGoogleRegister.ImageURL,
                UserTypeId=(int)eUser,
                IsActive=true
            };

            _context.Persons.Add(NewPerson);

            if (await _context.SaveChangesAsync() <= 0)
                return null;

            string UserType = ((eUsersType)NewPerson.UserTypeId).ToString();


            return new AuthenticatedPersonDTO()
            {
                FirstName = NewPerson.FirstName,
                LastName = NewPerson.LastName,
                ImageURL = NewPerson.ImageURL,
                PersonId = NewPerson.Id,
                UserType = UserType,
            };
        }

        private async Task<bool> SetRegisterInfo(Person person,PersonRegisterDTO personRegisterDTO,eUsersType userType)
        {
            string UserImage = personRegisterDTO.ImageURL is not null ? personRegisterDTO.ImageURL : DefaultImageURL;

            person.FirstName= personRegisterDTO.FirstName;
            person.LastName= personRegisterDTO.LastName;
            person.ImageURL = UserImage;
            person.CountryId = personRegisterDTO.CountryId;
           // person.GovernorateId = personRegisterDTO.GovernorateId;
            person.Phone = personRegisterDTO.PhoneNumber;
            person.IsActive = true;
            person.UserTypeId = (int)userType;
           


            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PersonRegisterDTO?> RegisterPerson(PersonRegisterDTO personRegisterDTO,int PersonId,eUsersType userType)
        {
            Person? person = await  _context.Persons.Where(P=>P.Id==PersonId).FirstOrDefaultAsync();

            if (person is null) return null;

            if (!await SetRegisterInfo(person, personRegisterDTO,userType))
                return null;

            string UserType = ((eUsersType)person.UserTypeId).ToString();

            return new PersonRegisterDTO()
            {
                FirstName = person.FirstName!,
                LastName = person.LastName!,
                ImageURL = person.ImageURL,
                UserType = UserType,
                CountryId = person.CountryId!.Value,
            };
        }

        public async Task<bool> IsThisEmailHasActivePerson(string Email)
        {
            bool IsExists = await  _context.Persons.Where(P => P.Email == Email && P.IsActive).AnyAsync();

            return IsExists;
        }

        public async Task<PreRegisterPersonResponseDTO> PreRegiseterPerson(PersonLoginDTO personLoginDTO, eUsersType eUser)
        {
            string EncryptPassword = _securityService.Encrypt(personLoginDTO.Password);

            var NewPerson = new Person()
            {
                Email = personLoginDTO.Email,
                Password = EncryptPassword,
                UserTypeId=(int)eUser,
                IsActive=false
            };

            _context.Persons.Add(NewPerson);

            if (await _context.SaveChangesAsync() <= 0)
                throw new Exception("Faild to add Person");

            return new PreRegisterPersonResponseDTO() 
            { 
                PersonId=NewPerson.Id,
                Email = NewPerson.Email,
            };
        }

        public async Task<PersonAccountInfoDTO> GetPersonAccountInfoById(int PersonId)
        {
            PersonAccountInfoDTO? person = await  _context.Persons.Where(P => P.Id == PersonId)
                                                                  .Select(P => new PersonAccountInfoDTO()
                                                                  {
                                                                      FullName = P.FirstName + " " + P.LastName,
                                                                      Email = P.Email,
                                                                      Phone = P.Phone,
                                                                      CountryName = P.Country.Name,
                                                                      Image=P.ImageURL
                                                                  })
                                                                  .FirstOrDefaultAsync();

            return person!;
        }

        public async Task<PersonProfileInfoDTO> GetPersonProfileInfoById(int PersonId)
        {
            PersonProfileInfoDTO? person = await _context.Persons.Where(P => P.Id == PersonId)
                                                                  .Select(P => new PersonProfileInfoDTO()
                                                                  {
                                                                      FullName = P.FirstName + " " + P.LastName,
                                                                      CountryName = P.Country.Name,
                                                                      Image = P.ImageURL
                                                                  })
                                                                  .FirstOrDefaultAsync();

            return person!;
        }

        public async Task<bool> SetPersonCompleteInfoForSeekerType(SeekerCompleteInfoDTO infoDTO,int PersonId)
        {
            int NumOfAffectedRows = await _context.Persons.Where(P => P.Id == PersonId)
                                                          .ExecuteUpdateAsync(s =>s
                                                           .SetProperty(p => p.Phone, infoDTO.Phone)
                                                           .SetProperty(p => p.CountryId, infoDTO.CountryId)
                                                           );

            return NumOfAffectedRows > 0;
        }

        public async Task<bool> ChangeImage(int PersonId, string ImageURL)
        {
            int NumOfAffectedRows = await _context.Persons.Where(P => P.Id == PersonId)
                                                          .ExecuteUpdateAsync(s => s
                                                           .SetProperty(p => p.ImageURL, ImageURL));
            return NumOfAffectedRows > 0;
        }

    }
}
