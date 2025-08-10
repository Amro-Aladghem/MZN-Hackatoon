using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.SecurityServices
{
    public class PasswordServices: ISecurityService
    {
        private readonly string Key;
        private readonly IConfiguration _confiquration;

        public PasswordServices(IConfiguration confiquration)
        {
            _confiquration = confiquration;
            Key = _confiquration.GetSection("EncryptKey").Value!;
        }
        public string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {

                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];



                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);



                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }



                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {

                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];



                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);



                using (var msDecrypt = new System.IO.MemoryStream(Convert.FromBase64String(cipherText)))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                {

                    return srDecrypt.ReadToEnd();
                }
            }
        }

        public bool VerifyEncrypt(string PasswordFromDb, string EnteredPassword)
        {
            if (Decrypt(PasswordFromDb) != EnteredPassword)
            {
                return false;
            }

            return true;
        }
    }
}
