﻿
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using System.Collections.Generic;

namespace ResoClassAPI.Services
{
    public class VideoService : IVideoService
    
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        public VideoService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
        }

        public async Task<long> CreateVideo(VideoDto newItem)
        {
            var currentUser = authService.GetCurrentUser();

            if (currentUser != null)
            {
                Video newvideo = mapper.Map<Video>(newItem);


                if (newItem.SubTopicId > 0)
                {
                    if (dbContext.SubTopics.Any(x => x.Id == newItem.SubTopicId))
                        newvideo.SubTopicId = newItem.SubTopicId;
                    else
                        throw new Exception("Invalid SubTopicId");
                }

                if (newItem.TopicId > 0)
                {
                    if (dbContext.Topics.Any(x => x.Id == newItem.TopicId))
                        newvideo.TopicId = newItem.TopicId;
                    else
                        throw new Exception("Invalid TopicId");
                }

                if (newItem.ChapterId > 0)
                {
                    if (dbContext.Chapters.Any(x => x.Id == newItem.ChapterId))
                        newvideo.ChapterId = newItem.ChapterId;
                    else
                        throw new Exception("Invalid ChapterId");
                }

                newvideo.IsActive = true;
                newvideo.CreatedBy = newvideo.ModifiedBy = currentUser.Name;
                newvideo.CreatedOn = newvideo.ModifiedOn = DateTime.Now;

                dbContext.Videos.Add(newvideo);
                await dbContext.SaveChangesAsync();
                return newvideo.Id;
            }
            return 0;
        }

        public async Task<bool> DeleteVideo(long Id)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Videos.FirstOrDefault(item => item.Id == Id);

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

        public async Task<List<VideoResponseDto>> GetAllVideos()
        {
            List<VideoResponseDto> dtoObjects = new List<VideoResponseDto>();
            var videosList = await Task.FromResult(dbContext.Videos.Where(item => item.IsActive == true).ToList());
            var chapters = await Task.FromResult(dbContext.Chapters.ToList());
            var topics = await Task.FromResult(dbContext.Topics.ToList());
            var subTopics = await Task.FromResult(dbContext.SubTopics.ToList());
            if (videosList != null && videosList.Count > 0)
            {
                foreach (var video in videosList)
                {
                    var dtoObject = mapper.Map<VideoResponseDto>(video);

                    if(video.ChapterId != null && chapters.Count > 0)
                        dtoObject.Chapter = chapters.Where(x => x.Id == video.ChapterId).FirstOrDefault().Name;

                    if (video.TopicId != null && topics.Count > 0)
                        dtoObject.Topic = topics.Where(x => x.Id == video.TopicId).FirstOrDefault().Name;

                    if (video.SubTopicId != null && subTopics.Count > 0)
                        dtoObject.SubTopic = subTopics.Where(x => x.Id == video.SubTopicId).FirstOrDefault().Name;

                    dtoObjects.Add(dtoObject);
                }
                return dtoObjects;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<VideoDto> GetVideo(long Id)
        {
            var vrVideos = await Task.FromResult(dbContext.Videos.FirstOrDefault(item => item.Id == Id && item.IsActive == true));
            if (vrVideos != null)
            {
                var dtoObject = mapper.Map<VideoDto>(vrVideos);
                //dtoObject.Role = dbContext.Roles.First(item => item.Id == user.RoleId).Name;
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<bool> UpdateVideo(VideoDto updatedItem)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Videos.FirstOrDefault(item => item.Id == updatedItem.Id && item.IsActive == true);
            if (currentUser != null)
            {
                if (existingItem != null)
                {
                    if (!string.IsNullOrEmpty(updatedItem.Title))
                        existingItem.Title = updatedItem.Title;

                    if (!string.IsNullOrEmpty(updatedItem.Description))
                        existingItem.Description = updatedItem.Description;

                    if (!string.IsNullOrEmpty(updatedItem.ThumbNail))
                        existingItem.ThumbNail = updatedItem.ThumbNail;

                    if (!string.IsNullOrEmpty(updatedItem.SourceUrl))
                        existingItem.SourceUrl = updatedItem.SourceUrl;

                    if (updatedItem.SubTopicId > 0)
                    {
                        if (dbContext.SubTopics.Any(x => x.Id == updatedItem.SubTopicId))
                            existingItem.SubTopicId = updatedItem.SubTopicId;
                        else
                            throw new Exception("Invalid SubTopicId");
                    }

                    if (updatedItem.TopicId > 0)
                    {
                        if (dbContext.Topics.Any(x => x.Id == updatedItem.TopicId))
                            existingItem.TopicId = updatedItem.TopicId;
                        else
                            throw new Exception("Invalid TopicId");
                    }

                    if (updatedItem.ChapterId > 0)
                    {
                        if (dbContext.Chapters.Any(x => x.Id == updatedItem.ChapterId))
                            existingItem.ChapterId = updatedItem.ChapterId;
                        else
                            throw new Exception("Invalid ChapterId");
                    }

                    if (existingItem.HomeDisplay != updatedItem.HomeDisplay)
                        existingItem.HomeDisplay = updatedItem.HomeDisplay;

                    existingItem.ModifiedBy = currentUser.Name;
                    existingItem.ModifiedOn = DateTime.Now;

                    await dbContext.SaveChangesAsync();
                    return true;
                }
            }
            
            return false;
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


        public async Task<bool> InsertVideos(List<VideoExcelRequestDto> videos)
        {
            try
            {
                var currentUser = authService.GetCurrentUser();
                foreach (var video in videos)
                {
                    long chapterId = 0;
                    long topicId = 0;
                    long subTopicId = 0;

                    long courseId = dbContext.Courses.Where(c => c.Name == video.Course && c.IsActive).Select(c => c.Id).FirstOrDefault();

                    if (courseId == 0)
                    {
                        throw new Exception($"Course '{video.Course}' not found in the database.");
                    }

                    long subjectId = dbContext.Subjects.Where(c => c.Name == video.Subject && c.CourseId == courseId && c.IsActive).Select(c => c.Id).FirstOrDefault();

                    if (subjectId == 0)
                    {
                        throw new Exception($"Course '{video.Subject}' not found in the database.");
                    }

                    if (!string.IsNullOrEmpty(video.Chapter))
                    {
                        chapterId = dbContext.Chapters.Where(c => c.Name == video.Chapter && c.SubjectId == subjectId && c.IsActive).Select(c => c.Id).FirstOrDefault();

                        if (chapterId == 0)
                        {
                            throw new Exception($"Chapter '{video.Chapter}' not found in the database.");
                        }
                    }

                    if (!string.IsNullOrEmpty(video.Topic))
                    {
                        topicId = dbContext.Topics.Where(c => c.Name == video.Topic && c.ChapterId == chapterId && c.IsActive).Select(c => c.Id).FirstOrDefault();

                        if (topicId == 0)
                        {
                            throw new Exception($"Topic '{video.Topic}' not found in the database.");
                        }
                    }

                    if (!string.IsNullOrEmpty(video.SubTopic))
                    {
                        subTopicId = dbContext.SubTopics.Where(c => c.Name == video.SubTopic && c.TopicId == topicId && c.IsActive).Select(c => c.Id).FirstOrDefault();

                        if (subTopicId == 0)
                        {
                            throw new Exception($"SubTopic '{video.SubTopic}' not found in the database.");
                        }
                    }

                    Video newVideo = new Video
                    {
                        Title = video.Title,
                        Description = !string.IsNullOrEmpty(video.Description) ? video.Description : string.Empty,
                        ThumbNail = video.Thumbnail,
                        HomeDisplay = video.HomeDisplay,
                        SourceUrl = video.SourceUrl,
                        IsActive = true,
                        CreatedBy = currentUser.Name,
                        CreatedOn = DateTime.Now,
                        ModifiedBy = currentUser.Name,
                        ModifiedOn = DateTime.Now
                    };

                    if (subTopicId > 0)
                        newVideo.SubTopicId = subTopicId;

                    if (topicId > 0)
                        newVideo.TopicId = topicId;

                    if (chapterId > 0)
                        newVideo.ChapterId = chapterId;
                    dbContext.Videos.Add(newVideo);

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
