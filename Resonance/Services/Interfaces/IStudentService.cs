using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IStudentService
    {
        Task<List<ChapterResponseDto>> GetRecommendedChaptersWithCourseId(long courseId);
        Task<List<SubjectDto>> GetSubjectsWithCourseId(long courseId);
        Task<List<ChapterResponseDto>> GetChaptersWithSubjectId(long subjectId);
        Task<List<VideoResponseDto>> GetVideosWithChapterId(long chapterId);
        Task<List<VideoResponseDto>> GetVideosWithTopicId(long topicId);
        Task<List<VideoResponseDto>> GetVideosWithSubTopicId(long subTopicId);
        Task<List<TopicDto>> GetTopicsWithChapterId(long chapterId);
    }
}
