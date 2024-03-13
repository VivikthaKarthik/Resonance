using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IAssessmentService
    {
        Task<List<AssessmentSessionDto>> GetAssessmentSessions();
        Task<AssessmentConfigurationDto> GetAssessmentConfiguration();
        Task<string> InsertQuestions(List<QuestionsDto> questions, QuestionsUploadRequestDto request);

        Task<QuestionResponseDto> GetQuestions(QuestionRequestDto requestDto);

        Task<bool> StartAssessment(long assessmentId);
        Task<bool> EndAssessment(long assessmentId);
        Task<bool> UpdateQuestionStatus(UpdateAssessmentStatusDto request);

        Task<AssessmentReportDto> GetAssessmentReport(long id);
    }
}
