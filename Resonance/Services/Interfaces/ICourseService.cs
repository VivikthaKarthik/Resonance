using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface ICourseService
    {
        Task<List<CourseDto>> GetAllCourses();
        Task<CourseDto> GetCourse(int courseId);
        Task<long> CreateCourse(CourseDto newItem);
        Task<bool> UpdateCourse(CourseDto updatedItem);
        Task<bool> DeleteCourse(int courseId);
    }
}
