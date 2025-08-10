using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ExternalServices
{
    public class WhatsAppLinkService
    {
        private readonly IConfiguration _configuration;
        public  WhatsAppLinkService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetContanctWhatsAppLink(string message)
        {
            string phoneNumber = _configuration.GetSection("phone").Value!;
            string encodeMessage = Uri.EscapeDataString(message);

            string WhatsAppLink = $"https://wa.me/{phoneNumber}?text={encodeMessage}";

            return WhatsAppLink;
        }
    }
}
