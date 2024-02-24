using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IAssessmentService
    {
        Task<bool> InsertQuestions(List<QuestionsDto> questions);
    }
}
