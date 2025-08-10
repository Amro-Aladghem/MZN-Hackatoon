using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Taskalayze
{
    public class TokenService(JwtOptions jwtOptions)
    {
        public string CreateToken(int UserId,string Role,int ?CountryId=0,bool IsHasUniversity=false,int?UniversityId=null)
        {
            int countryId = CountryId.HasValue ? CountryId.Value : 0;

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
                , SecurityAlgorithms.HmacSha256),

                Expires = DateTime.UtcNow.AddMinutes(jwtOptions.Lifetime),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.NameIdentifier,UserId.ToString()),
                    new (ClaimTypes.Role,Role),
                    new (ClaimTypes.Country,countryId.ToString()),
                    new("isHasUniversity",IsHasUniversity.ToString()),
                    new("universityId",UniversityId==null?"":UniversityId.ToString())
                })
            };

            var SecurtiyToken = tokenHandler.CreateToken(tokenDescriptor);

            var accessToken = tokenHandler.WriteToken(SecurtiyToken);

            return accessToken;
        }
    }
}
