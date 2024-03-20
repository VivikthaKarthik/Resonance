using AutoMapper;
using DocumentFormat.OpenXml.VariantTypes;
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

        public async Task<List<SubjectsViewDto>> GetAllSubjects()
        {
            List<SubjectsViewDto> dtoObjects = new List<SubjectsViewDto>();
            var subjects = await Task.FromResult(dbContext.VwSubjects.ToList());
            if (subjects != null && subjects.Count > 0)
                dtoObjects = mapper.Map<List<SubjectsViewDto>>(subjects);
            return dtoObjects;
        }

        public async Task<long> CreateSubject(SubjectDto subject)
        {
            var currentUser = authService.GetCurrentUser();

            if (currentUser != null)
            {
                Subject newSubject = mapper.Map<Subject>(subject);

                if (subject.ClassId > 0)
                {
                    if (!dbContext.Classes.Any(x => x.Id == subject.ClassId))
                        throw new Exception("Invalid ClassId");
                }
                else
                    throw new Exception("ClassId is missing");

                newSubject.ClassId = subject.ClassId;
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
                if (updatedSubject.ClassId > 0)
                {
                    if (dbContext.Classes.Any(x => x.Id == updatedSubject.ClassId))
                        existingItem.ClassId = updatedSubject.ClassId;
                    else
                        throw new Exception("Invalid ClassId");
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

        public async Task<List<SubjectDto>> GetSubjectsWithCourseId(long classId)
        {
            
            if (dbContext.Subjects.Any(item => item.ClassId == classId && item.IsActive))
            {
                var subjects = await Task.FromResult(dbContext.Subjects.Where(item => item.ClassId == classId && item.IsActive).ToList());
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

                    long classId = dbContext.Classes
                        .Where(c => c.Name == subjectDto.ClassName && c.CourseId == courseId)
                        .Select(c => c.Id)
                        .FirstOrDefault();

                    if (classId == 0)
                    {
                        throw new Exception($"Class '{subjectDto.ClassName}' not found in the database.");
                    }

                    // Insert the subject if it doesn't exist
                    Subject existingSubject = dbContext.Subjects.FirstOrDefault(s => s.Name == subjectDto.Name && s.IsActive);

                    if (existingSubject == null)
                    {
                        existingSubject = new Subject 
                        { 
                            Name = subjectDto.Name, 
                            IsActive = true,
                            ClassId = classId,
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
