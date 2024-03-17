using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IScheduledExamService
    {
        Task<string> InsertQuestions(List<QuestionsDto> questions, ScheduledExamRequestDto request);
        Task<List<ScheduledExamResponseDto>> GetScheduledExams();
        Task<List<QuestionsDto>> GetScheduledExamQuestions(long id);
    }
}
