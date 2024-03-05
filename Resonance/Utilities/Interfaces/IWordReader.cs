using ResoClassAPI.DTOs;

namespace ResoClassAPI.Utilities.Interfaces
{
    public interface IWordReader
    {
        Task<List<QuestionsDto>> ProcessDocument(IFormFile document);
    }
}
