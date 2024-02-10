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
        Task<List<ChapterResponseDto>> GetRecommendedChaptersWithCourseId(long courseId);
        Task<List<ChapterResponseDto>> GetChaptersWithSubjectId(long subjectId);
    }
}
