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
                newvideo.IsActive = true;
                newvideo.CreatedBy = newvideo.ModifiedBy = currentUser.Name;
                newvideo.CreatedOn = newvideo.ModifiedOn = DateTime.Now;

                dbContext.Videos.Add(newvideo);
                await dbContext.SaveChangesAsync();
                return newvideo.Id;
            }
            return 0;
        }

        public async Task<bool> DeleteVideo(int Id)
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

        public async Task<List<VideoDto>> GetAllVideos()
        {
            List<VideoDto> dtoObjects = new List<VideoDto>();
            var vid = await Task.FromResult(dbContext.Videos.Where(item => item.IsActive == true).ToList());
            if (vid != null && vid.Count > 0)
            {

                foreach (var vrVideo in vid)
                {
                    var dtoObject = mapper.Map<VideoDto>(vrVideo);
                    dtoObjects.Add(dtoObject);
                }
                return dtoObjects;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<VideoDto> GetVideo(int Id)
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
                    existingItem.SubTopicId = updatedItem.SubTopicId;
                    existingItem.TopicId = updatedItem.TopicId;
                    existingItem.ChapterId = updatedItem.ChapterId;
                    existingItem.TopicId = updatedItem.TopicId;
                    if (!string.IsNullOrEmpty(updatedItem.HomeDisplay))
                        existingItem.HomeDisplay = updatedItem.HomeDisplay;
                    existingItem.IsActive = true;
                    existingItem.ModifiedBy = currentUser.Name;
                    existingItem.ModifiedOn = DateTime.Now;

                    await dbContext.SaveChangesAsync();
                    return true;
                }
            }
            
            return false;
        }
    }
}