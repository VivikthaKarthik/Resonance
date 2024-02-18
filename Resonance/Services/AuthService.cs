using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ResoClassAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ResoClassContext dbContext;
        private IConfiguration _config;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthService(IConfiguration configuration, ResoClassContext _dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this._config = configuration;
            dbContext = _dbContext;
            _contextAccessor = httpContextAccessor;
        }
        public CurrentUser GetCurrentUser()
        {
            CurrentUser currentUser = new CurrentUser();

            if (_contextAccessor.HttpContext != null)
            {
                currentUser.UserId = Convert.ToInt64(_contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
                currentUser.Name = _contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                currentUser.Email = _contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                currentUser.Role = _contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                currentUser.DeviceId = _contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "DeviceId")?.Value;
            }
            return currentUser;
        }

        public async Task<string> AuthenticateWebUser(WebLoginDto userDto)
        {
            string token = string.Empty;
            string decryptedPassword = DecryptPassword(userDto.Password);

            var userDetails = dbContext.Users.FirstOrDefault(item =>
            (item.Email == userDto.UserName || item.PhoneNumber == userDto.UserName) && item.Password == decryptedPassword && item.IsActive == true);

            if (userDetails != null)
            {
                userDetails.LastLoginDate = DateTime.Now;
                await dbContext.SaveChangesAsync();

                var role = dbContext.Roles.FirstOrDefault(item => item.Id == userDetails.RoleId).Name;
                token = await GenerateToken( userDetails.Email, userDetails.FirstName + " " + userDetails.LastName, role, userDetails?.Id.ToString(), string.Empty);
            }
            return await Task.FromResult(token);
        }

        public async Task<StudentLoginResponseDto> AuthenticateWebStudent(WebLoginDto userDto)
        {
            StudentLoginResponseDto response = new StudentLoginResponseDto();
            string decryptedPassword = DecryptPassword(userDto.Password);

            var studentDetails = dbContext.Students.FirstOrDefault(item =>
            (item.MobileNumber == userDto.UserName || item.EmailAddress.ToLower() == userDto.UserName.ToLower() || item.AdmissionId.ToLower() == userDto.UserName.ToLower()) && item.Password == decryptedPassword && item.IsActive == true);

            if (studentDetails != null)
            {
                studentDetails.LastLoginDate = DateTime.Now;
                await dbContext.SaveChangesAsync();

                response.IsPasswordChangeRequired = studentDetails.IsPasswordChangeRequired.Value;
                response.Token = await GenerateToken(studentDetails.EmailAddress, studentDetails.Name, "", studentDetails?.Id.ToString(), string.Empty);
            }
            return await Task.FromResult(response);
        }

        public async Task<StudentLoginResponseDto> AuthenticateMobileStudent(MobileLoginDto userDto)
        {
            StudentLoginResponseDto response = new StudentLoginResponseDto();
            string decryptedPassword = DecryptPassword(userDto.Password);

            var studentDetails = dbContext.Students.FirstOrDefault(item =>
            (item.MobileNumber == userDto.UserName || item.EmailAddress.ToLower() == userDto.UserName.ToLower() || item.AdmissionId.ToLower() == userDto.UserName.ToLower()) && item.Password == decryptedPassword && item.IsActive == true);

            if (studentDetails != null)
            {
                studentDetails.DeviceId = userDto.DeviceId;
                studentDetails.Longitude = userDto.Longitude;
                studentDetails.Latitude = userDto.Latitude;
                studentDetails.FirebaseId = userDto.FireBaseId;
                studentDetails.LastLoginDate = DateTime.Now;

                await dbContext.SaveChangesAsync();
                response.IsPasswordChangeRequired = studentDetails.IsPasswordChangeRequired.Value;
                response.Token = await GenerateToken(studentDetails.EmailAddress, studentDetails.Name, "", studentDetails?.Id.ToString(), userDto.DeviceId);
            }
            return await Task.FromResult(response);
        }

        private async Task<string> GenerateToken(string email, string userName, string role, string userId, string deviceId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, userName), 
                new Claim(ClaimTypes.Role, role), 
                new Claim(ClaimTypes.Sid, userId),
                new Claim("DeviceId", deviceId)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims: claims,
                expires: !string.IsNullOrEmpty(deviceId) ? DateTime.Now.AddYears(1) : DateTime.Now.AddMinutes(20),
                signingCredentials: credentials
                );

            return await Task.FromResult("bearer " + new JwtSecurityTokenHandler().WriteToken(token));
        }

        public string DecryptPassword(string encryptedPassword)
        {
            string key = _config["SecretKey"];

            if (string.IsNullOrEmpty(key))
                return encryptedPassword;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.IV = new byte[16]; // Make sure IV is properly provided from the client side for better security

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);
                byte[] decryptedBytes;

                using (var msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            decryptedBytes = Encoding.UTF8.GetBytes(srDecrypt.ReadToEnd());
                        }
                    }
                }

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        public string EncryptPassword(string password)
        {
            string key = _config["SecretKey"];

            if (string.IsNullOrEmpty(key))
                return password;

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
