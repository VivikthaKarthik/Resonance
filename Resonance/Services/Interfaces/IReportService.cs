using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IReportService
    {
        //Task<List<SubjectWeightageDto>> GetSubjectsReport();
        ////Task<StudentReportDto> GetStudentReport();
        //Task<SubjectReportDto> GetSubjectReport(long id);
        //Task<ChapterReportDto> GetChapterReport(long id);

        Task<SubjectWiseTestDto> TrackYourProgressBySubject(long subjectId);
        Task<ChapterWiseTestDto> TrackYourProgressByChapter(long chapterId);
        Task<TopicWiseTestDto> TrackYourProgressByTopic(long topicId);
        Task<AssessmentReportDto> GetAssessmentReport(long assessmentId);
        Task<SubjectWiseTimeAnalysisDto> TimeSpentAnalysisBySubject(long subjectId);
        Task<ChapterWiseTimeAnalysisDto> TimeSpentAnalysisByChapter(long chapterId);
        Task<TopicWiseTimeAnalysisDto> TimeSpentAnalysisByTopic(long topicId);
        Task<SubjectWiseDifficultyLevelAnalysisDto> DifficultyLevelAnalysisBySubject(long subjectId);
        Task<ChapterWiseDifficultyLevelAnalysisDto> DifficultyLevelAnalysisByChapter(long chapterId);
        Task<TopicWiseDifficultyLevelAnalysisDto> DifficultyLevelAnalysisByTopic(long topicId);
    }
}
