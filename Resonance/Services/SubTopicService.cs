
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

        public async Task<List<SubTopicsViewDto>> GetAllSubTopics()
        {
            List<SubTopicsViewDto> dtoObjects = new List<SubTopicsViewDto>();
            var subTopics = await Task.FromResult(dbContext.VwSubTopics.ToList());
            if (subTopics != null && subTopics.Count > 0)
                dtoObjects = mapper.Map<List<SubTopicsViewDto>>(subTopics);
            return dtoObjects;
        }
        public async Task<SubTopicResponseDto> GetSubTopic(long subTopicId)
        {
            var query = from subtopic in dbContext.SubTopics
                        where subtopic.Id == subTopicId
                        join topic in dbContext.Topics on subtopic.TopicId equals topic.Id
                        join chapter in dbContext.Chapters on topic.ChapterId equals chapter.Id
                        join subject in dbContext.Subjects on chapter.SubjectId equals subject.Id
                        select new SubTopicResponseDto
                        {
                            Id = subtopic.Id,
                            Name = subtopic.Name,
                            Thumbnail = subtopic.Thumbnail,
                            SourceUrl = subtopic.SourceUrl,
                            Description = subtopic.Description,
                            HomeDisplay = subtopic.HomeDisplay,
                            ClassNotesUrl = subtopic.ClassNotesUrl,
                            ExtractUrl = subtopic.ExtractUrl,
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
                            SourceUrl = subtopic.SourceUrl,
                            Description = subtopic.Description,
                            HomeDisplay = subtopic.HomeDisplay,
                            ClassNotesUrl = subtopic.ClassNotesUrl,
                            ExtractUrl = subtopic.ExtractUrl,
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


        public async Task<List<SubTopicsViewDto>> GetVideosWithChapterId(long chapterId)
        {
            var subTopics = await Task.FromResult(dbContext.VwSubTopics.Where(item => item.ChapterId == chapterId).ToList());
            if (subTopics != null && subTopics.Count > 0)
                return mapper.Map<List<SubTopicsViewDto>>(subTopics);
            else
                return new List<SubTopicsViewDto>();
        }

        public async Task<List<SubTopicsViewDto>> GetVideosWithTopicId(long topicId)
        {
            var subTopics = await Task.FromResult(dbContext.VwSubTopics.Where(item => item.TopicId == topicId).ToList());
            if (subTopics != null && subTopics.Count > 0)
                return mapper.Map<List<SubTopicsViewDto>>(subTopics);
            else
                return new List<SubTopicsViewDto>();
        }
        public async Task<bool> InsertSubTopics(List<SubTopicExcelRequestDto> subTopics)
        {
            try
            {
                var currentUser = authService.GetCurrentUser();
                foreach (var subTopic in subTopics)
                {
                    // Get the course ID based on the course name
                    long courseId = dbContext.Courses.Where(c => c.Name == subTopic.Course && c.IsActive).Select(c => c.Id).FirstOrDefault();

                    if (courseId == 0)
                    {
                        throw new Exception($"Course '{subTopic.Course}' not found in the database.");
                    }

                    long classId = dbContext.Classes.Where(c => c.Name == subTopic.Class && c.CourseId == courseId && c.IsActive).Select(c => c.Id).FirstOrDefault();

                    if (classId == 0)
                    {
                        throw new Exception($"Class '{subTopic.Class}' not found in the database.");
                    }

                    long subjectId = dbContext.Subjects.Where(c => c.Name == subTopic.Subject && c.ClassId == classId && c.IsActive).Select(c => c.Id).FirstOrDefault();

                    if (subjectId == 0)
                    {
                        throw new Exception($"Subject '{subTopic.Subject}' not found in the database.");
                    }

                    long chapterId = dbContext.Chapters.Where(c => c.Name == subTopic.Chapter && c.SubjectId == subjectId && c.IsActive).Select(c => c.Id).FirstOrDefault();

                    if (chapterId == 0)
                    {
                        throw new Exception($"Chapter '{subTopic.Chapter}' not found in the database.");
                    }
                    long topicId = 0;
                    if (!string.IsNullOrEmpty(subTopic.Topic))
                    {
                        topicId = dbContext.Topics.Where(c => c.Name == subTopic.Topic && c.ChapterId == chapterId && c.IsActive).Select(c => c.Id).FirstOrDefault();

                        if (topicId == 0)
                        {
                            throw new Exception($"Topic '{subTopic.Topic}' not found in the database.");
                        }
                    }

                    // Insert the subject if it doesn't exist
                    SubTopic existingTopic = null;

                    if (topicId > 0)
                        existingTopic = dbContext.SubTopics.FirstOrDefault(s => s.Name == subTopic.Name && s.ChapterId == chapterId && s.TopicId == topicId && s.IsActive);
                    else 
                        existingTopic = dbContext.SubTopics.FirstOrDefault(s => s.Name == subTopic.Name && s.ChapterId == chapterId && s.IsActive);

                    if (existingTopic == null)
                    {
                        existingTopic = new SubTopic
                        {
                            Name = subTopic.Name,
                            SourceUrl = subTopic.SourceUrl,
                            HomeDisplay = subTopic.HomeDisplay,
                            Description = !string.IsNullOrEmpty(subTopic.Description) ? subTopic.Description : string.Empty,
                            Thumbnail = subTopic.Thumbnail,
                            ChapterId = chapterId,
                            TopicId = topicId > 0 ? topicId : null,
                            IsActive = true,
                            CreatedBy = currentUser.Name,
                            CreatedOn = DateTime.Now,
                            ModifiedBy = currentUser.Name,
                            ModifiedOn = DateTime.Now
                        };
                        dbContext.SubTopics.Add(existingTopic);

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

        public async Task<AttachmentsDto> GetAttachments(long id)
        {
            return new AttachmentsDto();
        }
    }
}
