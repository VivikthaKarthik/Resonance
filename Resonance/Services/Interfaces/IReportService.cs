using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IReportService
    {
        Task<List<SubjectWeightageDto>> GetSubjectsReport();
        Task<StudentReportDto> GetStudentReport();
        Task<SubjectReportDto> GetSubjectReport(long id);
        Task<ChapterReportDto> GetChapterReport(long id);
        Task<AssessmentReportDto> GetAssessmentReport(long id);
    }
}
