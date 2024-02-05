using Resonance.DTOs;

namespace Resonance.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> AuthenticateUser(LoginDto user);
    }
}
