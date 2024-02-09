using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface ITopicService
    {
        Task<List<TopicDto>> GetAllTopics();
        Task<TopicDto> GetTopic(int topicId);
        Task<long> CreateTopic(TopicDto newItem);
        Task<bool> UpdateTopic(TopicDto updatedItem);
        Task<bool> DeleteTopic(int topicId);
    }
}
