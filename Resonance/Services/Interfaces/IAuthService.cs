using ResoClassAPI.DTOs;
using ResoClassAPI.Models;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IAuthService
    {
        CurrentUser GetCurrentUser();
        Task<string> AuthenticateWebUser(WebLoginDto user);
        Task<StudentLoginResponseDto> AuthenticateWebStudent(WebLoginDto user);
        Task<StudentLoginResponseDto> AuthenticateMobileStudent(MobileLoginDto user);
    }
}
