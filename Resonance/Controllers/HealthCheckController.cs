using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace ResoClassAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {

        [HttpGet]
        public string Get()
        {
            var pwd = EncryptPassword("123", "Oks6Vw0C/1+eLTMegckT7xgIsVQkggod9JFAEZv0/pU=");
            return "ResoClass API is Running successfully\nVersion: 8\n" + pwd;
        }

        private string EncryptPassword(string password, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.IV = new byte[16]; // Make sure IV is properly provided for better security

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes;

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                        csEncrypt.Write(passwordBytes, 0, passwordBytes.Length);
                    }
                    encryptedBytes = msEncrypt.ToArray();
                }

                return Convert.ToBase64String(encryptedBytes);
            }
        }
    }
}
