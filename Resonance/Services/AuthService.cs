using Microsoft.IdentityModel.Tokens;
using ResoClass.DTOs;
using ResoClass.Services.Interfaces;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ResoClass.Services
{
    public class AuthService : IAuthService
    {
        //private readonly ConquerContext dbContext;
        private IConfiguration _config;

        public AuthService(IConfiguration configuration) //, ConquerContext _dbContext)
        {
            this._config = configuration;
            //dbContext = _dbContext;
        }

        public async Task<string> AuthenticateUser(WebLoginDto userDto)
        {
            string token = string.Empty;

            token = await GenerateToken(userDto.UserName, "Admin", "1");

            //var userDetails = dbContext.Users.FirstOrDefault(item => item.UserName == userDto.UserName && item.Password == userDto.Password);

            //if (userDetails != null)
            //{
            //    var role = dbContext.Roles.FirstOrDefault(item => item.Id == userDetails.RoleId).Name;
            //    token = await GenerateToken(userDetails.UserName, role, userDetails?.UserId);
            //}
            return await Task.FromResult(token);
        }

        private async Task<string> GenerateToken(string userName, string role, string userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim(ClaimTypes.Name, userName), new Claim(ClaimTypes.Role, role), new Claim(ClaimTypes.Sid, userId) };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: credentials
                );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
