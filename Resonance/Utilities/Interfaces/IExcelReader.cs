using ResoClassAPI.DTOs;

namespace ResoClassAPI.Utilities.Interfaces
{
    public interface IExcelReader
    {
        Task<List<StudentProfileDto>> ReadStudentsFromExcel(Stream stream);
        Task<List<SubjectDto>> ReadSubjectsFromExcel(Stream stream);
        Task<List<ChapterRequestDto>> ReadChaptersFromExcel(Stream stream);
        Task<bool> BulkUpload(IFormFile file, string tableName);
    }
}
