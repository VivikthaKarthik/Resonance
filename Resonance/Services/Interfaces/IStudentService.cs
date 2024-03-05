using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IStudentService
    {
        Task<List<StudentDto>> GetAllStudents();
        Task<StudentDto> GetStudent(long studentId);
        Task<long> CreateStudent(StudentDto newItem);
        Task<bool> UpdateStudent(StudentDto updatedItem);
        Task<bool> DeleteStudent(long chapterId);
        Task<StudentProfileDto> GetProfile();
        Task<bool> ChangePassword(string password);
        Task<bool> InsertStudents(List<StudentProfileDto> subjects);
    }
}
