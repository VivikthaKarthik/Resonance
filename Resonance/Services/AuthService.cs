using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
                currentUser.UserId = _contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
                currentUser.Name = _contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            }
            return currentUser;
        }

        public async Task<string> AuthenticateWebUser(WebLoginDto userDto)
        {
            string token = string.Empty;

            var userDetails = dbContext.ResoUsers.FirstOrDefault(item =>
            (item.Email == userDto.UserName || item.PhoneNumber == userDto.UserName) && item.Password == userDto.Password);

            if (userDetails != null)
            {
                var role = dbContext.Roles.FirstOrDefault(item => item.Id == userDetails.RoleId).Name;
                token = await GenerateToken(userDetails.Email, role, userDetails?.Id.ToString(), string.Empty);
            }
            return await Task.FromResult(token);
        }

        public async Task<string> AuthenticateMobileUser(MobileLoginDto userDto)
        {
            string token = string.Empty;

            var userDetails = dbContext.ResoUsers.FirstOrDefault(item =>
            (item.Email == userDto.UserName || item.PhoneNumber == userDto.UserName) && item.Password == userDto.Password);

            if (userDetails != null)
            {
                userDetails.DeviceId = userDto.DeviceId;
                userDetails.Longitude = userDto.Longitude;
                userDetails.Latitude = userDto.Latitude;
                userDetails.RegistrationId = userDto.RegistrationId;

                await dbContext.SaveChangesAsync();
                var role = dbContext.Roles.FirstOrDefault(item => item.Id == userDetails.RoleId).Name;
                token = await GenerateToken(userDetails.Email, role, userDetails?.Id.ToString(), userDto.DeviceId);
            }
            return await Task.FromResult(token);
        }

        private async Task<string> GenerateToken(string userName, string role, string userId, string deviceId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] 
            { 
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

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
