using ResoClassAPI.DTOs;
using ResoClassAPI.Models;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IAuthService
    {
        CurrentUser GetCurrentUser();
        Task<string> AuthenticateWebUser(WebLoginDto user);
        Task<string> AuthenticateWebStudent(WebLoginDto user);
        Task<string> AuthenticateMobileStudent(MobileLoginDto user);
    }
}
