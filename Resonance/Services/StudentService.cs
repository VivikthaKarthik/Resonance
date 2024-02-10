using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public async Task<long> CreateStudent(StudentDto student)
        {
            var currentUser = authService.GetCurrentUser();

            if (currentUser != null)
            {
                Student newStudent = mapper.Map<Student>(student);

                if (student.CourseId > 0)
                {
                    if (!dbContext.Courses.Any(x => x.Id == student.CourseId))
                        throw new Exception("Invalid CourseId");
                }
                else
                    throw new Exception("CourseId is missing");

                newStudent.IsActive = true;
                newStudent.CreatedBy = newStudent.ModifiedBy = currentUser.Name;
                newStudent.CreatedOn = newStudent.ModifiedOn = DateTime.Now;

                dbContext.Students.Add(newStudent);
                await dbContext.SaveChangesAsync();

                
                return newStudent.Id;
            }
            return 0;
        }

        public async Task<bool> DeleteStudent(long studentId)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Students.FirstOrDefault(item => item.Id == studentId && item.IsActive == true);

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

        public async Task<List<StudentDto>> GetAllStudents()
        {
            List<StudentDto> dtoObjects = new List<StudentDto>();
            var students = await Task.FromResult(dbContext.Students.Where(item => item.IsActive == true).ToList());
            if (students != null && students.Count > 0)
            {
                foreach (var student in students)
                {
                    var dtoObject = mapper.Map<StudentDto>(student);
                    dtoObjects.Add(dtoObject);
                }
                return dtoObjects;
            }
            else
                throw new Exception("Not Found");
        }



        public async Task<StudentDto> GetStudent(long studentId)
        {
            var student = await Task.FromResult(dbContext.Students.FirstOrDefault(item => item.Id == studentId && item.IsActive == true));
            if (student != null)
            {
                var dtoObject = mapper.Map<StudentDto>(student);
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<bool> UpdateStudent(StudentDto updatedStudent)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Students.FirstOrDefault(item => item.Id == updatedStudent.Id && item.IsActive == true);

            if (existingItem != null)
            {
                if (!string.IsNullOrEmpty(updatedStudent.AdmissionId))
                    existingItem.AdmissionId = updatedStudent.AdmissionId;
                existingItem.AdmissionDate = updatedStudent.AdmissionDate;
                if (!string.IsNullOrEmpty(updatedStudent.Name))
                    existingItem.Name = updatedStudent.Name;
                if (!string.IsNullOrEmpty(updatedStudent.FatherName))
                    existingItem.FatherName = updatedStudent.FatherName;
                if (!string.IsNullOrEmpty(updatedStudent.MotherName))
                    existingItem.MotherName = updatedStudent.MotherName;
                existingItem.DateOfBirth = updatedStudent.DateOfBirth;
                if (!string.IsNullOrEmpty(updatedStudent.AddressLine1))
                    existingItem.AddressLine1 = updatedStudent.AddressLine1;
                if (!string.IsNullOrEmpty(updatedStudent.AddressLine2))
                    existingItem.AddressLine2 = updatedStudent.AddressLine2;
                if (!string.IsNullOrEmpty(updatedStudent.Landmark))
                    existingItem.Landmark = updatedStudent.Landmark;
                existingItem.CityId = updatedStudent.CityId;
                existingItem.StateId = updatedStudent.StateId;
                if (!string.IsNullOrEmpty(updatedStudent.PinCode))
                    existingItem.PinCode = updatedStudent.PinCode;
                if (!string.IsNullOrEmpty(updatedStudent.BranchId))
                    existingItem.BranchId = updatedStudent.BranchId;
                if (!string.IsNullOrEmpty(updatedStudent.Gender))
                    existingItem.Gender = updatedStudent.Gender;
                existingItem.CourseId = updatedStudent.CourseId;
                if (!string.IsNullOrEmpty(updatedStudent.MobileNumber))
                    existingItem.MobileNumber = updatedStudent.MobileNumber;
                if (!string.IsNullOrEmpty(updatedStudent.AlternateMobileNumber))
                    existingItem.AlternateMobileNumber = updatedStudent.AlternateMobileNumber;
                if (!string.IsNullOrEmpty(updatedStudent.EmailAddress))
                    existingItem.EmailAddress = updatedStudent.EmailAddress;
                if (!string.IsNullOrEmpty(updatedStudent.DeviceId))
                    existingItem.DeviceId = updatedStudent.DeviceId;
                if (!string.IsNullOrEmpty(updatedStudent.FirebaseId))
                    existingItem.FirebaseId = updatedStudent.FirebaseId;
                if (!string.IsNullOrEmpty(updatedStudent.Password))
                    existingItem.Password = updatedStudent.Password;
                if (!string.IsNullOrEmpty(updatedStudent.Longitude))
                    existingItem.Longitude = updatedStudent.Longitude;
                if (!string.IsNullOrEmpty(updatedStudent.Latitude))
                    existingItem.Latitude = updatedStudent.Latitude;
                existingItem.LastLoginDate = updatedStudent.LastLoginDate;

                existingItem.ModifiedBy = currentUser.Name;
                existingItem.ModifiedOn = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return true;

            }
            return false;
        }
    }
}
