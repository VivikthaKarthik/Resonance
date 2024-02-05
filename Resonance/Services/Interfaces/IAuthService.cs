using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> AuthenticateWebUser(WebLoginDto user);
        Task<string> AuthenticateMobileUser(MobileLoginDto user);
    }
}
