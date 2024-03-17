using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IScheduledExamService
    {
        Task<string> InsertQuestions(List<QuestionsDto> questions, ScheduledExamRequestDto request);
        Task<List<ScheduledExamResponseDto>> GetScheduledExams();
        Task<List<ScheduledExamResponseDto>> GetExams();
        Task<List<QuestionsDto>> GetScheduledExamQuestions(long id);
        Task<ScheduledExamQuestionsResponseDto> GetQuestions(long id);
        Task<bool> StartAssessment(long examId);
        Task<bool> EndAssessment(long examId);
        Task<bool> UpdateQuestionStatus(UpdateScheduledExamQuestionStatusDto request);
        Task<bool> DeleteQuestions(List<long> ids);
    }
}
