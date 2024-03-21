using AutoMapper;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;

namespace Resonance
{
    public class MapperConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                //User Mapper
                config.CreateMap<UserDto, User>().ForMember(x => x.RoleId, opt => opt.Ignore()).ForMember(x => x.Role, opt => opt.Ignore());
                config.CreateMap<User, UserDto>().ForMember(x => x.Role, opt => opt.Ignore());

                //Subject Mapper
                config.CreateMap<SubjectDto, Subject>();
                config.CreateMap<Subject, SubjectDto>();
                config.CreateMap<VwSubject, SubjectsViewDto>();

                //Chapter Mapper
                config.CreateMap<ChapterRequestDto, Chapter>();
                config.CreateMap<Chapter, ChapterResponseDto>();
                config.CreateMap<VwChapter, ChaptersViewDto>();

                //Topic Mapper
                config.CreateMap<TopicDto, Topic>();
                config.CreateMap<Topic, TopicDto>();
                config.CreateMap<Topic, TopicResponseDto>();
                config.CreateMap<VwTopic, TopicsViewDto>();

                //Sub-Topic Mapper
                config.CreateMap<SubTopicDto, SubTopic>();
                config.CreateMap<SubTopic, SubTopicResponseDto>();
                config.CreateMap<VwSubTopic, SubTopicsViewDto>();

                //Video Mapper
                config.CreateMap<VideoDto, Video>();
                config.CreateMap<Video, VideoDto>();
                config.CreateMap<Video, VideoResponseDto>();

                //Course Mapper
                config.CreateMap<CourseDto, Course>();
                config.CreateMap<Course, CourseDto>();

                //Class Mapper
                config.CreateMap<ClassDto, Class>();
                config.CreateMap<Class, ClassDto>();
                config.CreateMap<VwClass, ClassesViewDto>();

                //Student Mapper
                config.CreateMap<Student, StudentProfileDto>();
                config.CreateMap<Student, StudentDto>();
                config.CreateMap<StudentDto, Student>();

                //QuestionBank Mapper
                config.CreateMap<QuestionBank, QuestionData>();
                config.CreateMap<QuestionBank, QuestionsDto>();
                config.CreateMap<QuestionBank, QuestionKeyDto>();
                config.CreateMap<VwQuestionBank, QuestionData>();

                //Assessment Mapper
                config.CreateMap<AssessmentConfiguration, AssessmentConfigurationDto>();
                config.CreateMap<AssessmentSession, AssessmentSessionDto>();
                config.CreateMap<AssessmentLevel, AssessmentLevelDto>();

                //ScheduledExam Mapper
                config.CreateMap<ScheduledExam, ScheduledExamResponseDto>(); 
                config.CreateMap<ScheduledExamQuestion, QuestionsDto>();
                config.CreateMap<ScheduledExamQuestion, ScheduledExamQuestionData>();
            });
            return mapperConfig;
        }
    }
}
