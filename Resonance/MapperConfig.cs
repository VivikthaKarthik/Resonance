﻿using AutoMapper;
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

                //Chapter Mapper
                config.CreateMap<ChapterDto, Chapter>();
                config.CreateMap<Chapter, ChapterDto>();

                //Topic Mapper
                config.CreateMap<TopicDto, Topic>();
                config.CreateMap<Topic, TopicDto>();

                //Sub-Topic Mapper
                config.CreateMap<SubTopicDto, SubTopic>();
                config.CreateMap<SubTopic, SubTopicDto>();

                //Video Mapper
                config.CreateMap<VideoDto, Video>();
                config.CreateMap<Video, VideoDto>();

                //Course Mapper
                config.CreateMap<CourseDto, Course>();
                config.CreateMap<Course, CourseDto>();
            });
            return mapperConfig;
        }
    }
}
