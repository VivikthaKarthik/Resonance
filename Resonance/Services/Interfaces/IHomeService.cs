using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IHomeService
    {
        Task<SearchResponseDto> SearchItems(string text);
        Task<List<ListItemDto>> GetListItems(string itemName, string? parentName, long? parentId);
    }
}
