using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;

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
            var student = await Task.FromResult(dbContext.Students.FirstOrDefault(item => item.Id == currentUser.UserId && item.IsActive == true));
            if (student != null)
            {
                var dtoObject = mapper.Map<StudentProfileDto>(student);

                if(student.CityId > 0)
                    dtoObject.City = dbContext.Cities.Where(x => x.Id == student.CityId).First().Name;

                if (student.StateId > 0)
                    dtoObject.State = dbContext.Cities.Where(x => x.Id == student.StateId).First().Name;

                if (student.CourseId > 0)
                    dtoObject.CourseName = dbContext.Courses.Where(x => x.Id == student.CourseId).First().Name;
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

        public async Task<bool> InsertStudents(List<StudentProfileDto> students)
        {
            try
            {
                var currentUser = authService.GetCurrentUser();
                foreach (var studentProfile in students)
                {
                    long cityId = dbContext.Cities
                        .Where(c => c.Name == studentProfile.City)
                        .Select(c => c.Id)
                        .FirstOrDefault();

                    long stateId = dbContext.States
                        .Where(c => c.Name == studentProfile.State)
                        .Select(c => c.Id)
                        .FirstOrDefault();

                    // Get the course ID based on the course name
                    long courseId = dbContext.Courses
                        .Where(c => c.Name == studentProfile.CourseName)
                        .Select(c => c.Id)
                        .FirstOrDefault();

                    if (cityId == 0)
                        throw new Exception($"Course '{studentProfile.City}' not found in the database.");

                    if (stateId == 0)
                        throw new Exception($"State '{studentProfile.State}' not found in the database.");

                    if (courseId == 0)
                        throw new Exception($"City '{studentProfile.CourseName}' not found in the database.");

                    if(string.IsNullOrEmpty(studentProfile.EmailAddress) || !ValidateEmail(studentProfile.EmailAddress))
                        throw new Exception($"'{studentProfile.EmailAddress}' is not a valid Email.");

                    if (string.IsNullOrEmpty(studentProfile.MobileNumber) || !ValidatePhoneNumber(studentProfile.MobileNumber))
                        throw new Exception($"'{studentProfile.MobileNumber}' is not a valid Phone Number.");

                    if (!string.IsNullOrEmpty(studentProfile.AlternateMobileNumber))
                    {
                        if(!ValidatePhoneNumber(studentProfile.AlternateMobileNumber))
                            throw new Exception($"'{studentProfile.AlternateMobileNumber}' is not a valid Phone Number.");
                    }

                    if (string.IsNullOrEmpty(studentProfile.Name))
                        throw new Exception("Name Field is mandatory.");

                    if (string.IsNullOrEmpty(studentProfile.AdmissionId))
                        throw new Exception("AdmissionId Field is mandatory.");

                    if (studentProfile.AdmissionDate == null || studentProfile.AdmissionDate == DateTime.MinValue)
                        throw new Exception("AdmissionDate Field is mandatory.");

                    if (string.IsNullOrEmpty(studentProfile.FatherName))
                        throw new Exception("FatherName Field is mandatory.");

                    if (string.IsNullOrEmpty(studentProfile.MotherName))
                        throw new Exception("MotherName Field is mandatory.");

                    if (studentProfile.DateOfBirth == null || studentProfile.DateOfBirth == DateTime.MinValue)
                        throw new Exception("DateOfBirth Field is mandatory.");

                    if (string.IsNullOrEmpty(studentProfile.AddressLine1))
                        throw new Exception("AddressLine1 Field is mandatory.");

                    if (string.IsNullOrEmpty(studentProfile.PinCode))
                        throw new Exception("PinCode Field is mandatory.");

                    if (string.IsNullOrEmpty(studentProfile.Gender))
                        throw new Exception("Gender Field is mandatory.");

                    // Insert the subject if it doesn't exist
                    Student existingStudent = dbContext.Students.FirstOrDefault(s => s.Name == studentProfile.Name && s.EmailAddress == studentProfile.EmailAddress && s.IsActive);

                    if (existingStudent == null)
                    {
                        existingStudent = new Student
                        {
                            Name = studentProfile.Name,
                            AdmissionId = studentProfile.AdmissionId,
                            AdmissionDate = studentProfile.AdmissionDate,
                            FatherName = studentProfile.FatherName,
                            MotherName = studentProfile.MotherName,
                            DateOfBirth = studentProfile.DateOfBirth,
                            AddressLine1 = studentProfile.AddressLine1,
                            AddressLine2 = studentProfile.AddressLine2,
                            Landmark = studentProfile.Landmark,
                            CityId = cityId,
                            StateId = stateId,
                            PinCode = studentProfile.PinCode,
                            Gender = studentProfile.Gender,
                            CourseId = courseId,
                            MobileNumber = studentProfile.MobileNumber,
                            AlternateMobileNumber = studentProfile.AlternateMobileNumber,
                            EmailAddress = studentProfile.EmailAddress,
                            IsPasswordChangeRequired = true,
                            Password = "Reso@123",
                            ProfilePicture = studentProfile.ProfilePicture,
                            IsActive = true,
                            CreatedBy = currentUser.Name,
                            CreatedOn = DateTime.Now,
                            ModifiedBy = currentUser.Name,
                            ModifiedOn = DateTime.Now
                        };
                        dbContext.Students.Add(existingStudent);
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

        private bool ValidatePhoneNumber(string phoneNumber)
        {
            // Regular expression for a typical phone number pattern
            string phonePattern = @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$";

            // Check if the phone number matches the pattern
            return Regex.IsMatch(phoneNumber, phonePattern);
        }

        private bool ValidateEmail(string email)
        {
            // Regular expression for a typical email pattern
            string emailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";

            // Check if the email matches the pattern
            return Regex.IsMatch(email, emailPattern);
        }

    }
}
