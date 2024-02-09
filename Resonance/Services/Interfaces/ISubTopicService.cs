using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface ISubTopicService
    {
        Task<List<SubTopicDto>> GetAllSubTopics();
        Task<SubTopicDto> GetSubTopic(int topicId);
        Task<long> CreateSubTopic(SubTopicDto newItem);
        Task<bool> UpdateSubTopic(SubTopicDto updatedItem);
        Task<bool> DeleteSubTopic(int topicId);
    }
}
