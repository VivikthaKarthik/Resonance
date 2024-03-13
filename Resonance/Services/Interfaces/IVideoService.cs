using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IVideoService
    {
        Task<List<VideoResponseDto>> GetAllVideos();
        Task<VideoDto> GetVideo(long topicId);
        Task<long> CreateVideo(VideoDto newItem);
        Task<bool> UpdateVideo(VideoDto updatedItem);
        Task<bool> DeleteVideo(long topicId);
        Task<List<VideoResponseDto>> GetVideosWithChapterId(long chapterId);
        Task<List<VideoResponseDto>> GetVideosWithTopicId(long topicId);
        Task<List<VideoResponseDto>> GetVideosWithSubTopicId(long subTopicId);
        Task<bool> InsertVideos(List<VideoExcelRequestDto> videos);
    }
}
