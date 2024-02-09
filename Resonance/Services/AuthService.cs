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
                currentUser.Email = _contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                currentUser.Role = _contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                currentUser.DeviceId = _contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "DeviceId")?.Value;
            }
            return currentUser;
        }

        public async Task<string> AuthenticateWebUser(WebLoginDto userDto)
        {
            string token = string.Empty;

            var userDetails = dbContext.Users.FirstOrDefault(item =>
            (item.Email == userDto.UserName || item.PhoneNumber == userDto.UserName) && item.Password == userDto.Password && item.IsActive == true);

            if (userDetails != null)
            {
                userDetails.LastLoginDate = DateTime.Now;
                await dbContext.SaveChangesAsync();

                var role = dbContext.Roles.FirstOrDefault(item => item.Id == userDetails.RoleId).Name;
                token = await GenerateToken(userDetails.FirstName + " " + userDetails.LastName, userDetails.Email, role, userDetails?.Id.ToString(), string.Empty);
            }
            return await Task.FromResult(token);
        }

        public async Task<StudentLoginResponseDto> AuthenticateWebStudent(WebLoginDto userDto)
        {
            StudentLoginResponseDto response = new StudentLoginResponseDto();

            var studentDetails = dbContext.Students.FirstOrDefault(item =>
            (item.MobileNumber == userDto.UserName || item.EmailAddress.ToLower() == userDto.UserName.ToLower() || item.AdmissionId.ToLower() == userDto.UserName.ToLower()) && item.Password == userDto.Password && item.IsActive == true);

            if (studentDetails != null)
            {
                studentDetails.LastLoginDate = DateTime.Now;
                await dbContext.SaveChangesAsync();

                response.StudentId = studentDetails.Id;
                response.Name = studentDetails.Name;

                if (dbContext.Courses.Any(x => x.Id == studentDetails.CourseId))
                {
                    var courseDetails = dbContext.Courses.Where(x => x.Id == studentDetails.CourseId).First();
                    response.CourseId = courseDetails.Id;
                    response.CourseName = courseDetails.Name;
                }

                response.Token = await GenerateToken(studentDetails.Name, studentDetails.EmailAddress, "", studentDetails?.Id.ToString(), string.Empty);
            }
            return await Task.FromResult(response);
        }

        public async Task<StudentLoginResponseDto> AuthenticateMobileStudent(MobileLoginDto userDto)
        {
            StudentLoginResponseDto response = new StudentLoginResponseDto();

            var studentDetails = dbContext.Students.FirstOrDefault(item =>
            (item.MobileNumber == userDto.UserName || item.EmailAddress.ToLower() == userDto.UserName.ToLower() || item.AdmissionId.ToLower() == userDto.UserName.ToLower()) && item.Password == userDto.Password && item.IsActive == true);

            if (studentDetails != null)
            {
                studentDetails.DeviceId = userDto.DeviceId;
                studentDetails.Longitude = userDto.Longitude;
                studentDetails.Latitude = userDto.Latitude;
                studentDetails.FirebaseId = userDto.FireBaseId;
                studentDetails.LastLoginDate = DateTime.Now;

                await dbContext.SaveChangesAsync();

                response.StudentId = studentDetails.Id;
                response.Name = studentDetails.Name;

                if (dbContext.Courses.Any(x => x.Id == studentDetails.CourseId))
                {
                    var courseDetails = dbContext.Courses.Where(x => x.Id == studentDetails.CourseId).First();
                    response.CourseId = courseDetails.Id;
                    response.CourseName = courseDetails.Name;
                }
                response.Token = await GenerateToken(studentDetails.Name, studentDetails.EmailAddress, "", studentDetails?.Id.ToString(), userDto.DeviceId);
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
    }
}
