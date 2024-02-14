using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IStudentService
    {
        Task<bool> ChangePassword(string password);
    }
}
