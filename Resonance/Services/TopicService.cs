
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using System.Collections.Generic;

namespace ResoClassAPI.Services
{
    public class TopicService : ITopicService
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        public TopicService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
        }

        public async Task<TopicDto> GetTopic(long topicId)
        {
            var topic = await Task.FromResult(dbContext.Topics.FirstOrDefault(item => item.Id == topicId && item.IsActive == true));
            if (topic != null)
            {
                var dtoObject = mapper.Map<TopicDto>(topic);
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<List<TopicDto>> GetAllTopics()
        {
            List<TopicDto> dtoObjects = new List<TopicDto>();
            var topics = await Task.FromResult(dbContext.Topics.Where(item => item.IsActive == true).ToList());
            if (topics != null && topics.Count > 0)
            {

                //foreach (var topic in topics)
                //{
                dtoObjects = mapper.Map<List<TopicDto>>(topics);
                    //dtoObjects.Add(dtoObject);
                //}
                return dtoObjects;
            }
            else
                throw new Exception("Not Found");
        }      
        public async Task<long> CreateTopic(TopicDto topic)
        {
            var currentUser = authService.GetCurrentUser();

            if (currentUser != null)
            {
                Topic newTopic = mapper.Map<Topic>(topic);

                if (topic.ChapterId > 0)
                {
                    if (dbContext.Chapters.Any(x => x.Id == topic.ChapterId))
                        newTopic.ChapterId = topic.ChapterId;
                    else
                        throw new Exception("Invalid ChapterId");
                }
                else
                    throw new Exception("ChapterId is missing");

                newTopic.IsActive = true;
                newTopic.CreatedBy = newTopic.ModifiedBy = currentUser.Name;
                newTopic.CreatedOn = newTopic.ModifiedOn = DateTime.Now;

                dbContext.Topics.Add(newTopic);
                await dbContext.SaveChangesAsync();
                return newTopic.Id;
            }
            return 0;
        }
        public async Task<bool> UpdateTopic(TopicDto updatedTopic)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Topics.FirstOrDefault(item => item.Id == updatedTopic.Id && item.IsActive == true);

            if (currentUser != null)
            {
                if (existingItem != null)
                {
                    if (!string.IsNullOrEmpty(updatedTopic.Name))
                        existingItem.Name = updatedTopic.Name;

                    if (!string.IsNullOrEmpty(updatedTopic.Description))
                        existingItem.Description = updatedTopic.Description;

                    if (!string.IsNullOrEmpty(updatedTopic.Thumbnail))
                        existingItem.Thumbnail = updatedTopic.Thumbnail;

                    if (updatedTopic.ChapterId > 0)
                    {
                        if (dbContext.Chapters.Any(x => x.Id == updatedTopic.ChapterId))
                            existingItem.ChapterId = updatedTopic.ChapterId;
                        else
                            throw new Exception("Invalid ChapterId");
                    }

                    existingItem.ModifiedBy = currentUser.Name;
                    existingItem.ModifiedOn = DateTime.Now;
                    await dbContext.SaveChangesAsync();
                    return true;
                }
            }
            
            return false;
        }
        public async Task<bool> DeleteTopic(long topicId)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Topics.FirstOrDefault(item => item.Id == topicId && item.IsActive == true);

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
        public async Task<TopicResponseDto> GetTopicById(long topicId)
        {
            var query = from topic in dbContext.Topics
                        where topic.Id == topicId
                        join chapter in dbContext.Chapters on topic.ChapterId equals chapter.Id
                        join subject in dbContext.Subjects on chapter.SubjectId equals subject.Id
                        select new TopicResponseDto()
                        {
                            Id = topic.Id,
                            Name = topic.Name,
                            Description = !string.IsNullOrEmpty(topic.Description) ? topic.Description : string.Empty,
                            Thumbnail = topic.Thumbnail,
                            SubjectId = subject.Id,
                            SubjectName = subject.Name,
                            ChapterId = chapter.Id,
                            ChapterName = chapter.Name
                        };

            var result = query.ToList();
            if (result != null && result.Count > 0)
                return result.First();
            else
                throw new Exception("Not Found");
        }
        public async Task<List<TopicDto>> GetTopicsWithChapterId(long chapterId)
        {
            var topics = await Task.FromResult(dbContext.Topics.Where(item => item.ChapterId == chapterId && item.IsActive == true).ToList());
            if (topics != null && topics.Count > 0)
                return mapper.Map<List<TopicDto>>(topics);
            else
                throw new Exception("Not Found");
        }
        public async Task<bool> InsertTopics(List<TopicExcelRequestDto> topics)
        {
            try
            {
                var currentUser = authService.GetCurrentUser();
                foreach (var topicDto in topics)
                {
                    // Get the course ID based on the course name
                    long courseId = dbContext.Courses.Where(c => c.Name == topicDto.Course && c.IsActive).Select(c => c.Id).FirstOrDefault();

                    if (courseId == 0)
                    {
                        throw new Exception($"Course '{topicDto.Course}' not found in the database.");
                    }

                    long subjectId = dbContext.Subjects.Where(c => c.Name == topicDto.Subject && c.CourseId == courseId && c.IsActive).Select(c => c.Id).FirstOrDefault();

                    if (subjectId == 0)
                    {
                        throw new Exception($"Subject '{topicDto.Subject}' not found in the database.");
                    }

                    long chapterId = dbContext.Chapters.Where(c => c.Name == topicDto.Chapter && c.SubjectId == subjectId && c.IsActive).Select(c => c.Id).FirstOrDefault();

                    if (chapterId == 0)
                    {
                        throw new Exception($"Chapter '{topicDto.Chapter}' not found in the database.");
                    }

                    // Insert the subject if it doesn't exist
                    Topic existingTopic = dbContext.Topics.FirstOrDefault(s => s.Name == topicDto.Name && s.ChapterId == chapterId && s.IsActive);

                    if (existingTopic == null)
                    {
                        existingTopic = new Topic
                        {
                            Name = topicDto.Name,
                            Description = !string.IsNullOrEmpty(topicDto.Description) ? topicDto.Description : string.Empty,
                            Thumbnail = topicDto.Thumbnail,
                            ChapterId = chapterId,
                            IsActive = true,
                            CreatedBy = currentUser.Name,
                            CreatedOn = DateTime.Now,
                            ModifiedBy = currentUser.Name,
                            ModifiedOn = DateTime.Now
                        };
                        dbContext.Topics.Add(existingTopic);

                    }
                }
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
