using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentProfileDto> GetProfile();
        Task<bool> ChangePassword(string password);
        Task<bool> InsertStudents(List<StudentProfileDto> subjects);
    }
}
