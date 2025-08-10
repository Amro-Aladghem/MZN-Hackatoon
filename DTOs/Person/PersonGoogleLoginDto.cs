using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Person
{
    public class PersonGoogleLoginDto
    {
        public string IdToken { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
