using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IAssessmentService
    {
        Task<string> InsertQuestions(List<QuestionsDto> questions, string? chapter, string? topic, string? subTopic);

        Task<QuestionResponseDto> GetQuestions(QuestionRequestDto requestDto);
    }
}
