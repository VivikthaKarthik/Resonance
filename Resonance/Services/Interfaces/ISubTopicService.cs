using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface ISubTopicService
    {
        Task<List<SubTopicsViewDto>> GetAllSubTopics();
        Task<SubTopicResponseDto> GetSubTopic(long subTopicId);
        Task<List<SubTopicResponseDto>> GetByTopicId(long topicId);
        Task<long> CreateSubTopic(SubTopicDto newItem);
        Task<bool> UpdateSubTopic(SubTopicDto updatedItem);
        Task<bool> DeleteSubTopic(long subTopicId);
        Task<List<SubTopicsViewDto>> GetVideosWithChapterId(long chapterId);
        Task<List<SubTopicsViewDto>> GetVideosWithTopicId(long topicId);
        Task<bool> InsertSubTopics(List<SubTopicExcelRequestDto> subTopics);

        Task<AttachmentsDto> GetAttachments(long id);
    }
}
