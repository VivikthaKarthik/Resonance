using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;

namespace ResoClassAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        public StudentService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
        }

        public async Task<List<ChapterResponseDto>> GetChaptersWithSubjectId(long subjectId)
        {
            var query = from chapter in dbContext.Chapters
                        where chapter.SubjectId == subjectId
                        join subject in dbContext.Subjects on chapter.SubjectId equals subject.Id
                        select new ChapterResponseDto
                        {
                            Id = chapter.Id,
                            Name = chapter.Name,
                            Thumbnail = chapter.Thumbnail,
                            SubjectId = subject.Id,
                            SubjectName = subject.Name,
                            IsRecommended = chapter.IsRecommended,
                        };

            var result = query.ToList();

            if (result != null && result.Count > 0)
                return result;
            else
                throw new Exception("Not Found");
        }

        public async Task<List<ChapterResponseDto>> GetRecommendedChaptersWithCourseId(long courseId)
        {
            var query = from subjectCourse in dbContext.SubjectCourses
            where subjectCourse.CourseId == courseId
                        join subject in dbContext.Subjects on subjectCourse.SubjectId equals subject.Id
                        join chapter in dbContext.Chapters on subject.Id equals chapter.SubjectId
                        where chapter.IsRecommended == true
                        select new ChapterResponseDto()
                        {
                            Id = chapter.Id,
                            Name = chapter.Name,
                            SubjectId = subject.Id,
                            SubjectName = subject.Name,
                            Thumbnail = chapter.Thumbnail,
                            IsRecommended = chapter.IsRecommended
                        };

            var result = query.ToList();
            if (result != null)
            {
                var dtoObject = result;
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<List<SubjectDto>> GetSubjectsWithCourseId(long courseId)
        {

            var query = from subjectCourse in dbContext.SubjectCourses
                        where subjectCourse.CourseId == courseId
                        join subject in dbContext.Subjects on subjectCourse.SubjectId equals subject.Id
                        select new Subject
                        {
                            Id = subject.Id,
                            Name = subject.Name,
                            Thumbnail = subject.Thumbnail
                        };

            var result = query.ToList();
            if (result != null)
            {
                var dtoObject = mapper.Map<List<SubjectDto>>(result);
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<List<VideoResponseDto>> GetVideosWithChapterId(long chapterId)
        {
            var videos = await Task.FromResult(dbContext.Videos.Where(item => item.ChapterId == chapterId && item.IsActive == true).ToList());
            if (videos != null && videos.Count > 0)
                return mapper.Map<List<VideoResponseDto>>(videos);
            else
                throw new Exception("Not Found");
        }

        public async Task<List<VideoResponseDto>> GetVideosWithTopicId(long topicId)
        {
            var videos = await Task.FromResult(dbContext.Videos.Where(item => item.TopicId == topicId && item.IsActive == true).ToList());
            if (videos != null && videos.Count > 0)
                return mapper.Map<List<VideoResponseDto>>(videos);
            else
                throw new Exception("Not Found");
        }

        public async Task<List<VideoResponseDto>> GetVideosWithSubTopicId(long subTopicId)
        {
            var videos = await Task.FromResult(dbContext.Videos.Where(item => item.SubTopicId == subTopicId && item.IsActive == true).ToList());
            if (videos != null && videos.Count > 0)
                return mapper.Map<List<VideoResponseDto>>(videos);
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
    }
}
