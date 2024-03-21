using AutoMapper;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;

namespace ResoClassAPI.Services
{
    public class ClassService : IClassService
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        public ClassService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
        }

        public async Task<long> CreateClass(ClassDto request)
        {
            var currentUser = authService.GetCurrentUser();

            if (currentUser != null)
            {
                if (request.CourseId <= 0 || !dbContext.Courses.Any(x => x.Id == request.CourseId))
                    throw new Exception("Invalid Course Id");

                Class newClass = mapper.Map<Class>(request);
                newClass.IsActive = true;
                newClass.CreatedBy = newClass.ModifiedBy = currentUser.Name;
                newClass.CreatedOn = newClass.ModifiedOn = DateTime.Now;

                dbContext.Classes.Add(newClass);
                await dbContext.SaveChangesAsync();

                return newClass.Id;
            }
            return 0;
        }

        public async Task<bool> DeleteClass(long classId)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Classes.FirstOrDefault(item => item.Id == classId && item.IsActive == true);

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

        public async Task<List<ClassesViewDto>> GetAllClasses()
        {
            List<ClassesViewDto> dtoObjects = new List<ClassesViewDto>();
            var classes = await Task.FromResult(dbContext.VwClasses.ToList());
            if (classes != null && classes.Count > 0)
                dtoObjects = mapper.Map<List<ClassesViewDto>>(classes);
            return dtoObjects;
        }

        public async Task<ClassDto> GetClass(long classId)
        {
            var course = await Task.FromResult(dbContext.Classes.FirstOrDefault(item => item.Id == classId && item.IsActive == true));
            if (course != null)
            {
                var dtoObject = mapper.Map<ClassDto>(course);
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<bool> UpdateClass(ClassDto updatedClass)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Classes.FirstOrDefault(item => item.Id == updatedClass.Id && item.IsActive == true);

            if (existingItem != null)
            {
                if (!string.IsNullOrEmpty(updatedClass.Name))
                    existingItem.Name = updatedClass.Name;

                if (!string.IsNullOrEmpty(updatedClass.Thumbnail))
                    existingItem.Thumbnail = updatedClass.Thumbnail;

                if(updatedClass.CourseId > 0 && updatedClass.CourseId != existingItem.CourseId)
                {
                    if (dbContext.Courses.Any(x => x.Id == updatedClass.CourseId))
                        existingItem.CourseId = updatedClass.CourseId;
                    else
                        throw new Exception("Invalid Course Id");
                }

                existingItem.ModifiedBy = currentUser.Name;
                existingItem.ModifiedOn = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;

        }

        public async Task<bool> InsertClassesAndLinkToCourses(List<ClassDto> classes)
        {
            try
            {
                var currentUser = authService.GetCurrentUser();
                foreach (var classDto in classes)
                {
                    // Get the course ID based on the course name
                    long courseId = dbContext.Courses
                        .Where(c => c.Name == classDto.Course)
                        .Select(c => c.Id)
                        .FirstOrDefault();

                    if (courseId == 0)
                    {
                        throw new Exception($"Course '{classDto.Course}' not found in the database.");
                    }

                    // Insert the subject if it doesn't exist
                    Class existingClass = dbContext.Classes.FirstOrDefault(s => s.Name == classDto.Name && s.IsActive);

                    if (existingClass == null)
                    {
                        existingClass = new Class
                        {
                            Name = classDto.Name,
                            IsActive = true,
                            Thumbnail = !string.IsNullOrEmpty(classDto.Thumbnail) ? classDto.Thumbnail : "NA",
                            CourseId = courseId,
                            CreatedBy = currentUser.Name,
                            CreatedOn = DateTime.Now,
                            ModifiedBy = currentUser.Name,
                            ModifiedOn = DateTime.Now
                        };
                        dbContext.Classes.Add(existingClass);
                    }
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
