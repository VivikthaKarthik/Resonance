﻿using AutoMapper;
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


        public async Task<StudentProfileDto> GetProfile()
        {
            var currentUser = authService.GetCurrentUser();
            var subject = await Task.FromResult(dbContext.Students.FirstOrDefault(item => item.Id == currentUser.UserId && item.IsActive == true));
            if (subject != null)
            {
                var dtoObject = mapper.Map<StudentProfileDto>(subject);

                if (dtoObject.CourseId > 0)
                    dtoObject.CourseName = dbContext.Courses.Where(x => x.Id == dtoObject.CourseId).First().Name;
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<bool> ChangePassword(string password)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Students.FirstOrDefault(item => item.Id == currentUser.UserId && item.IsActive == true);

            if (existingItem != null)
            {
                existingItem.Password = authService.DecryptPassword(password);
                existingItem.ModifiedBy = currentUser.Name;
                existingItem.ModifiedOn = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
