using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IChapterService
    {
        Task<List<ChapterDto>> GetAllChapters();
        Task<ChapterDto> GetChapter(int chapterId);
        Task<long> CreateChapter(ChapterDto newItem);
        Task<bool> UpdateChapter(ChapterDto updatedItem);
        Task<bool> DeleteChapter(int chapterId);
    }
}
