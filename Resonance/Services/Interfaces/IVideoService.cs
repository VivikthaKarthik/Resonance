using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IVideoService
    {
        Task<List<VideoDto>> GetAllVideos();
        Task<VideoDto> GetVideo(int topicId);
        Task<long> CreateVideo(VideoDto newItem);
        Task<bool> UpdateVideo(VideoDto updatedItem);
        Task<bool> DeleteVideo(int topicId);
    }
}
