
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using System.Collections.Generic;

namespace ResoClassAPI.Services
{
    public class SubTopicService : ISubTopicService
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        public SubTopicService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
        }

        public async Task<List<SubTopicResponseDto>> GetAllSubTopics()
        {
            List<SubTopicResponseDto> dtoObjects = new List<SubTopicResponseDto>();
            var topics = await Task.FromResult(dbContext.SubTopics.Where(item => item.IsActive == true).ToList());
            if (topics != null && topics.Count > 0)
            {

                foreach (var topic in topics)
                {
                    var dtoObject = mapper.Map<SubTopicResponseDto>(topic);
                    dtoObjects.Add(dtoObject);
                }
                return dtoObjects;
            }
            else
                throw new Exception("Not Found");
        }
        public async Task<SubTopicResponseDto> GetSubTopic(long subTopicId)
        {
            var query = from subtopic in dbContext.SubTopics
                        where subtopic.Id == subTopicId
                        join topic in dbContext.Topics on subtopic.TopicId equals topic.Id
                        join chapter in dbContext.Chapters on topic.ChapterId equals chapter.Id
                        join subjectChapter in dbContext.SubjectChapters on chapter.Id equals subjectChapter.ChapterId
                        join subject in dbContext.Subjects on subjectChapter.SubjectId equals subject.Id
                        select new SubTopicResponseDto
                        {
                            Id = subtopic.Id,
                            Name = subtopic.Name,
                            Thumbnail = subtopic.Thumbnail,
                            TopicId = topic.Id,
                            TopicName = topic.Name,
                            ChapterId = chapter.Id,
                            ChapterName = chapter.Name,
                            SubjectId = subject.Id,
                            SubjectName = subject.Name,
                        };

            var result = query.ToList();

            if (result != null && result.Count > 0)
                return result.First();
            else
                throw new Exception("Not Found");
        }

        public async Task<List<SubTopicResponseDto>> GetByTopicId(long topicId)
        {
            var query = from subtopic in dbContext.SubTopics
                        join topic in dbContext.Topics on subtopic.TopicId equals topic.Id
                        where topic.Id == topicId
                        select new SubTopicResponseDto
                        {
                            Id = subtopic.Id,
                            Name = subtopic.Name,
                            Thumbnail = subtopic.Thumbnail,
                            TopicId = topic.Id,
                            TopicName = topic.Name
                        };

            var result = query.ToList();

            if (result != null && result.Count > 0)
                return result;
            else
                throw new Exception("Not Found");
        }

        public async  Task<long>CreateSubTopic(SubTopicDto subTopic)
        {
            var currentUser = authService.GetCurrentUser();

            if (currentUser != null)
            {
                SubTopic newSbTopic = mapper.Map<SubTopic>(subTopic);

                if (subTopic.TopicId > 0)
                {
                    if (dbContext.Topics.Any(x => x.Id == subTopic.TopicId))
                        newSbTopic.TopicId = subTopic.TopicId;
                    else
                        throw new Exception("Invalid TopicId");
                }
                else
                    throw new Exception("TopicId is missing");

                newSbTopic.IsActive = true;
                newSbTopic.CreatedBy = newSbTopic.ModifiedBy = currentUser.Name;
                newSbTopic.CreatedOn = newSbTopic.ModifiedOn = DateTime.Now;

                dbContext.SubTopics.Add(newSbTopic);
                await dbContext.SaveChangesAsync();
                return newSbTopic.Id;
            }
            return 0;
        }
        public async Task<bool> UpdateSubTopic(SubTopicDto updatedSbTopic)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.SubTopics.FirstOrDefault(item => item.Id == updatedSbTopic.Id && item.IsActive == true);
            if (currentUser != null)
            {
                if (existingItem != null)
                {
                    if (!string.IsNullOrEmpty(updatedSbTopic.Name))
                        existingItem.Name = updatedSbTopic.Name;
                    if (!string.IsNullOrEmpty(updatedSbTopic.Thumbnail))
                        existingItem.Thumbnail = updatedSbTopic.Thumbnail;

                    if (updatedSbTopic.TopicId > 0)
                    {
                        if (dbContext.Topics.Any(x => x.Id == updatedSbTopic.TopicId))
                            existingItem.TopicId = updatedSbTopic.TopicId;
                        else
                            throw new Exception("Invalid SubjectId");
                    }
                    existingItem.ModifiedBy = currentUser.Name;
                    existingItem.ModifiedOn = DateTime.Now;

                    await dbContext.SaveChangesAsync();
                    return true;
                }
            }
            
            return false;
        }


        public async Task<bool> DeleteSubTopic(long subTopicId)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.SubTopics.FirstOrDefault(item => item.Id == subTopicId);

            if (existingItem != null)
            {
                existingItem.IsActive = false;
                existingItem.ModifiedBy = currentUser.Name;
                existingItem.ModifiedOn = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        

        

       
    }
}
