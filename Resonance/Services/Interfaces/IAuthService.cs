using ResoClassAPI.DTOs;
using ResoClassAPI.Models;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IAuthService
    {
        CurrentUser GetCurrentUser();
        Task<string> AuthenticateWebUser(WebLoginDto user);
        Task<string> AuthenticateMobileUser(MobileLoginDto user);
    }
}
