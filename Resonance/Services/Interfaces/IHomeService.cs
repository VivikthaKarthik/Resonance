using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IHomeService
    {
        Task<CurriculumCountResponseDto> GetCurriculumCount();
        Task<SearchResponseDto> SearchItems(string text);
        Task<List<ListItemDto>> GetListItems(string itemName, string? parentName, long? parentId);
    }
}
