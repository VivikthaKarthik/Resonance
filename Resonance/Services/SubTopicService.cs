
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

        public async Task<List<SubTopicDto>> GetAllSubTopics()
        {
            List<SubTopicDto> dtoObjects = new List<SubTopicDto>();
            var topics = await Task.FromResult(dbContext.SubTopics.Where(item => item.IsActive == true).ToList());
            if (topics != null && topics.Count > 0)
            {

                foreach (var topic in topics)
                {
                    var dtoObject = mapper.Map<SubTopicDto>(topic);
                    dtoObjects.Add(dtoObject);
                }
                return dtoObjects;
            }
            else
                throw new Exception("Not Found");
        }
        public async Task<SubTopicDto> GetSubTopic(int subTopicId)
        {
            var subTopic = await Task.FromResult(dbContext.SubTopics.FirstOrDefault(item => item.Id == subTopicId && item.IsActive == true));
            if (subTopic != null)
            {
                var dtoObject = mapper.Map<SubTopicDto>(subTopic);
                //dtoObject.Role = dbContext.Roles.First(item => item.Id == user.RoleId).Name;
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }
        public async  Task<long>CreateSubTopic(SubTopicDto subTopic)
        {
            var currentUser = authService.GetCurrentUser();

            if (currentUser != null)
            {
                SubTopic newSbTopic = mapper.Map<SubTopic>(subTopic);
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
                    existingItem.TopicId = updatedSbTopic.TopicId;
                    if (!string.IsNullOrEmpty(updatedSbTopic.Thumbnail))
                        existingItem.Thumbnail = updatedSbTopic.Thumbnail;
                    existingItem.IsActive = true;
                    existingItem.ModifiedBy = currentUser.Name;
                    existingItem.ModifiedOn = DateTime.Now;

                    await dbContext.SaveChangesAsync();
                    return true;
                }
            }
            
            return false;
        }


        public async Task<bool> DeleteSubTopic(int topicId)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.SubTopics.FirstOrDefault(item => item.Id == topicId);

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
