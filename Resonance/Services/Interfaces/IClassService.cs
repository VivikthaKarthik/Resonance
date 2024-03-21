using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IClassService
    {
        Task<List<ClassesViewDto>> GetAllClasses();
        Task<ClassDto> GetClass(long classId);
        Task<long> CreateClass(ClassDto newItem);
        Task<bool> UpdateClass(ClassDto updatedItem);
        Task<bool> DeleteClass(long courseId);
        Task<bool> InsertClassesAndLinkToCourses(List<ClassDto> classes);
    }
}
