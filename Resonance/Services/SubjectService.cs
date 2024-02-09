using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using System.Collections.Generic;

namespace ResoClassAPI.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        public SubjectService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
        }

        public async Task<List<SubjectDto>> GetAllSubjects()
        {
            List<SubjectDto> dtoObjects = new List<SubjectDto>();
            var subjects = await Task.FromResult(dbContext.Subjects.Where(item => item.IsActive == true).ToList());
            if (subjects != null && subjects.Count > 0)
            {

                foreach (var subject in subjects)
                {
                    var dtoObject = mapper.Map<SubjectDto>(subject);
                    dtoObjects.Add(dtoObject);
                }
                return dtoObjects;
            }
            else
                throw new Exception("Not Found");
           
           
        }

        public async Task<long> CreateSubject(SubjectDto subject)
        {
            var currentUser = authService.GetCurrentUser();

            if (currentUser != null)
            {
                Subject newSubject = mapper.Map<Subject>(subject);
                newSubject.IsActive = true;
                newSubject.CreatedBy = newSubject.ModifiedBy = currentUser.Name;
                newSubject.CreatedOn = newSubject.ModifiedOn = DateTime.Now;

                dbContext.Subjects.Add(newSubject);
                await dbContext.SaveChangesAsync();

                return newSubject.Id;
            }
            return 0;
        }

        public async Task<bool> UpdateSubject(SubjectDto updatedSubject)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Subjects.FirstOrDefault(item => item.Id == updatedSubject.Id && item.IsActive == true);

            if (existingItem != null)
            {
                if (!string.IsNullOrEmpty(updatedSubject.Name))
                    existingItem.Name = updatedSubject.Name;

                if (!string.IsNullOrEmpty(updatedSubject.Thumbnail))
                    existingItem.Thumbnail = updatedSubject.Thumbnail;

                existingItem.IsActive = true;
                existingItem.ModifiedBy = currentUser.Name;
                existingItem.ModifiedOn = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }



        public async Task<bool> DeleteSubject(int subjectId)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Subjects.FirstOrDefault(item => item.Id == subjectId);

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

        public async Task<SubjectDto> GetSubject(int subjectId)
        {
            var subject = await Task.FromResult(dbContext.Subjects.FirstOrDefault(item => item.Id == subjectId && item.IsActive == true));
            if (subject != null)
            {
                var dtoObject = mapper.Map<SubjectDto>(subject);
                //dtoObject.Role = dbContext.Roles.First(item => item.Id == user.RoleId).Name;
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }
    }
}
