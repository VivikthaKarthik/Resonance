using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using System.Collections.Generic;

namespace ResoClassAPI.Services
{
    public class CourseService : ICourseService
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        public CourseService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
        }

        public async Task<long> CreateCourse(CourseDto course)
        {
            var currentUser = authService.GetCurrentUser();

            if (currentUser != null)
            {
                Course newCourse = mapper.Map<Course>(course);
                newCourse.IsActive = true;
                newCourse.CreatedBy = newCourse.ModifiedBy = currentUser.Name;
                newCourse.CreatedOn = newCourse.ModifiedOn = DateTime.Now;

                dbContext.Courses.Add(newCourse);
                await dbContext.SaveChangesAsync();

                return newCourse.Id;
            }
            return 0;
        }

        public async Task<bool> DeleteCourse(long courseId)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Courses.FirstOrDefault(item => item.Id == courseId && item.IsActive == true);

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

        public async Task<List<CourseDto>> GetAllCourses()
        {
            List<CourseDto> dtoObjects = new List<CourseDto>();
            var courses = await Task.FromResult(dbContext.Courses.Where(item => item.IsActive == true).ToList());
            if (courses != null && courses.Count > 0)
            {

                foreach (var course in courses)
                {
                    var dtoObject = mapper.Map<CourseDto>(course);
                    dtoObjects.Add(dtoObject);
                }
                return dtoObjects;
            }
            else
                throw new Exception("Not Found");

        }

        public async Task<CourseDto> GetCourse(long courseId)
        {
            var course = await Task.FromResult(dbContext.Courses.FirstOrDefault(item => item.Id == courseId && item.IsActive == true));
            if (course != null)
            {
                var dtoObject = mapper.Map<CourseDto>(course);
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<bool> UpdateCourse(CourseDto updatedCourse)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Courses.FirstOrDefault(item => item.Id == updatedCourse.Id && item.IsActive == true);

            if (existingItem != null)
            {
                if (!string.IsNullOrEmpty(updatedCourse.Name))
                    existingItem.Name = updatedCourse.Name;

                if (!string.IsNullOrEmpty(updatedCourse.Thumbnail))
                    existingItem.Thumbnail = updatedCourse.Thumbnail;

                existingItem.ModifiedBy = currentUser.Name;
                existingItem.ModifiedOn = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;

        }
    }
}
