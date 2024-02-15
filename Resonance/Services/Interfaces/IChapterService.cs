using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IChapterService
    {
        Task<List<ChapterResponseDto>> GetAllChapters();
        Task<ChapterResponseDto> GetChapter(long chapterId);
        Task<long> CreateChapter(ChapterRequestDto newItem);
        Task<bool> UpdateChapter(ChapterRequestDto updatedItem);
        Task<bool> DeleteChapter(long chapterId);
        Task<List<RecommendedChapterResponseDto>> GetRecommendedChapters();
        Task<List<ChapterResponseDto>> GetChaptersWithSubjectId(long subjectId);
        Task<bool> InsertChaptersAndLinkToSubjects(List<ChapterRequestDto> chapters);
    }
}
