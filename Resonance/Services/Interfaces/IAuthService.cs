using ResoClass.DTOs;

namespace ResoClass.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> AuthenticateUser(WebLoginDto user);
    }
}
