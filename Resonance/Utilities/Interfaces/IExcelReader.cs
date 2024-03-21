using ResoClassAPI.DTOs;

namespace ResoClassAPI.Utilities.Interfaces
{
    public interface IExcelReader
    {
        Task<List<StudentProfileDto>> ReadStudentsFromExcel(Stream stream);
        Task<List<SubjectDto>> ReadSubjectsFromExcel(Stream stream);
        Task<List<ClassDto>> ReadClassesFromExcel(Stream stream);
        Task<List<CourseDto>> ReadCoursesFromExcel(Stream stream);
        Task<List<ChapterExcelRequestDto>> ReadChaptersFromExcel(Stream stream);
        Task<List<TopicExcelRequestDto>> ReadTopicsFromExcel(Stream stream);
        Task<bool> BulkUpload(IFormFile file, string tableName);
        Task<List<VideoExcelRequestDto>> ReadVideosFromExcel(Stream stream);
        Task<List<SubTopicExcelRequestDto>> ReadSubTopicsFromExcel(Stream stream);
    }
}
