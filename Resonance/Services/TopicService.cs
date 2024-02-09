﻿
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

        public async Task<List<TopicDto>> GetAllTopics()
        {
            List<TopicDto> dtoObjects = new List<TopicDto>();
            var topics = await Task.FromResult(dbContext.Topics.Where(item => item.IsActive == true).ToList());
            if (topics != null && topics.Count > 0)
            {

                foreach (var topic in topics)
                {
                    var dtoObject = mapper.Map<TopicDto>(topic);
                    dtoObjects.Add(dtoObject);
                }
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
                    existingItem.ChapterId = updatedTopic.ChapterId;
                    if (!string.IsNullOrEmpty(updatedTopic.Thumbnail))
                        existingItem.Thumbnail = updatedTopic.Thumbnail;
                    existingItem.IsActive = true;
                    if (!string.IsNullOrEmpty(currentUser.Name))
                        existingItem.ModifiedBy = currentUser.Name;
                    existingItem.ModifiedOn = DateTime.Now;

                    await dbContext.SaveChangesAsync();
                    return true;
                }
            }
            
            return false;
        }
        public async Task<bool> DeleteTopic(int topicId)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Topics.FirstOrDefault(item => item.Id == topicId);

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

        public async Task<TopicDto> GetTopic(int topicId)
        {
            var topic = await Task.FromResult(dbContext.Topics.FirstOrDefault(item => item.Id == topicId && item.IsActive == true));
            if (topic != null)
            {
                var dtoObject = mapper.Map<TopicDto>(topic);
                //dtoObject.Role = dbContext.Roles.First(item => item.Id == user.RoleId).Name;
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }





    }
}