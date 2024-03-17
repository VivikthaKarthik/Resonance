using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

                if (subject.CourseId > 0)
                {
                    if (!dbContext.Courses.Any(x => x.Id == subject.CourseId))
                        throw new Exception("Invalid CourseId");
                }
                else
                    throw new Exception("CourseId is missing");

                newSubject.CourseId = subject.CourseId;
                newSubject.IsActive = true;
                newSubject.CreatedBy = newSubject.ModifiedBy = currentUser.Name;
                newSubject.CreatedOn = newSubject.ModifiedOn = DateTime.Now;

                dbContext.Subjects.Add(newSubject);
                await dbContext.SaveChangesAsync();

                //SubjectCourse course = new SubjectCourse();
                //course.CourseId = subject.CourseId;
                //course.SubjectId = newSubject.Id;
                //course.IsActive = true;
                //course.CreatedBy = course.ModifiedBy = currentUser.Name;
                //course.CreatedOn = course.ModifiedOn = DateTime.Now;
                //dbContext.SubjectCourses.Add(course);
                //await dbContext.SaveChangesAsync();

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
                if (updatedSubject.CourseId > 0)
                {
                    if (dbContext.Courses.Any(x => x.Id == updatedSubject.CourseId))
                        existingItem.CourseId = updatedSubject.CourseId;
                    else
                        throw new Exception("Invalid CourseId");
                }

                if (!string.IsNullOrEmpty(updatedSubject.Name))
                    existingItem.Name = updatedSubject.Name;

                existingItem.ModifiedBy = currentUser.Name;
                existingItem.ModifiedOn = DateTime.Now;
                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteSubject(long subjectId)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Subjects.FirstOrDefault(item => item.Id == subjectId && item.IsActive == true);

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

        public async Task<SubjectDto> GetSubject(long subjectId)
        {
            var subject = await Task.FromResult(dbContext.Subjects.FirstOrDefault(item => item.Id == subjectId && item.IsActive == true));
            if (subject != null)
            {
                var dtoObject = mapper.Map<SubjectDto>(subject);
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<List<SubjectDto>> GetSubjectsWithCourseId(long courseId)
        {
            
            if (dbContext.Subjects.Any(item => item.CourseId == courseId && item.IsActive))
            {
                var subjects = await Task.FromResult(dbContext.Subjects.Where(item => item.CourseId == courseId && item.IsActive).ToList());
                var dtoObject = mapper.Map<List<SubjectDto>>(subjects);
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<bool> InsertSubjectsAndLinkToCourses(List<SubjectDto> subjects)
        {
            try
            {
                var currentUser = authService.GetCurrentUser();
                foreach (var subjectDto in subjects)
                {
                    // Get the course ID based on the course name
                    long courseId = dbContext.Courses
                        .Where(c => c.Name == subjectDto.CourseName)
                        .Select(c => c.Id)
                        .FirstOrDefault();

                    if (courseId == 0)
                    {
                        throw new Exception($"Course '{subjectDto.CourseName}' not found in the database.");
                    }

                    // Insert the subject if it doesn't exist
                    Subject existingSubject = dbContext.Subjects.FirstOrDefault(s => s.Name == subjectDto.Name && s.IsActive);

                    if (existingSubject == null)
                    {
                        existingSubject = new Subject 
                        { 
                            Name = subjectDto.Name, 
                            IsActive = true,
                            CourseId = courseId,
                            ColorCode = subjectDto.ColorCode,                            
                            CreatedBy = currentUser.Name,
                            CreatedOn = DateTime.Now,
                            ModifiedBy = currentUser.Name,
                            ModifiedOn = DateTime.Now
                        };
                        dbContext.Subjects.Add(existingSubject);
                    }

                    // Link the subject to the course
                    //existingSubject.SubjectCourses.Add(
                    //    new SubjectCourse
                    //    {
                    //        CourseId = courseId,
                    //        IsActive = true,
                    //        CreatedBy = currentUser.Name,
                    //        CreatedOn = DateTime.Now,
                    //        ModifiedBy = currentUser.Name,
                    //        ModifiedOn = DateTime.Now
                    //    });
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
