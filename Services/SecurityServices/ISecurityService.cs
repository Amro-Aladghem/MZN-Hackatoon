using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Services.SecurityServices
{
    public interface  ISecurityService
    {
        public string Encrypt(string plainText);
        public string Decrypt(string cipherText);
        public bool VerifyEncrypt(string DbValue, string CheckValue);
    }
}
