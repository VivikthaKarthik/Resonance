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
                    dtoObject.State = dbContext.States.Where(x => x.Id == student.StateId).First().Name;

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

                    if (stateId == 0)
                        throw new Exception($"State '{studentProfile.State}' not found in the database.");

                    if (courseId == 0)
                        throw new Exception($"City '{studentProfile.CourseName}' not found in the database.");


                    if (cityId == 0)
                    {
                        City newCity = new City();
                        newCity.Name = studentProfile.City;
                        newCity.StateId = stateId;
                        newCity.IsActive = true;
                        newCity.CreatedBy = currentUser.Name;
                        newCity.CreatedOn = DateTime.Now;
                        newCity.ModifiedBy = currentUser.Name;
                        newCity.ModifiedOn = DateTime.Now;

                        dbContext.Cities.Add(newCity);
                        await dbContext.SaveChangesAsync();

                        cityId = newCity.Id;

                    }

                    if (string.IsNullOrEmpty(studentProfile.EmailAddress) || !ValidateEmail(studentProfile.EmailAddress))
                        throw new Exception($"'{studentProfile.EmailAddress}' is not a valid Email.");

                    if (string.IsNullOrEmpty(studentProfile.MobileNumber) || !ValidatePhoneNumber(studentProfile.MobileNumber))
                        throw new Exception($"'{studentProfile.MobileNumber}' is not a valid Phone Number.");

                    if (!string.IsNullOrEmpty(studentProfile.AlternateMobileNumber))
                    {
                        if (!ValidatePhoneNumber(studentProfile.AlternateMobileNumber))
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
                    Student existingStudent = dbContext.Students.FirstOrDefault(s => s.AdmissionId.ToLower() == studentProfile.AdmissionId.ToLower() && s.IsActive);
                    bool isStudentExist = dbContext.Students.Any(s => s.AdmissionId.ToLower() == studentProfile.AdmissionId.ToLower() && s.IsActive);
                    if (!isStudentExist)
                    {
                        existingStudent = new Student();
                        existingStudent.IsPasswordChangeRequired = true;
                        existingStudent.Password = "Reso@123";
                        existingStudent.IsActive = true;
                        existingStudent.CreatedBy = currentUser.Name;
                        existingStudent.CreatedOn = DateTime.Now;
                    }
                    else
                    {
                        existingStudent = dbContext.Students.FirstOrDefault(s => s.AdmissionId.ToLower() == studentProfile.AdmissionId.ToLower() && s.IsActive);
                    }

                    existingStudent.Name = studentProfile.Name;
                    existingStudent.AdmissionId = studentProfile.AdmissionId;
                    existingStudent.AdmissionDate = studentProfile.AdmissionDate;
                    existingStudent.FatherName = studentProfile.FatherName;
                    existingStudent.MotherName = studentProfile.MotherName;
                    existingStudent.DateOfBirth = studentProfile.DateOfBirth;
                    existingStudent.AddressLine1 = studentProfile.AddressLine1;
                    existingStudent.AddressLine2 = studentProfile.AddressLine2;
                    existingStudent.Landmark = studentProfile.Landmark;
                    existingStudent.CityId = cityId;
                    existingStudent.StateId = stateId;
                    existingStudent.PinCode = studentProfile.PinCode;
                    existingStudent.Gender = studentProfile.Gender;
                    existingStudent.CourseId = courseId;
                    existingStudent.MobileNumber = studentProfile.MobileNumber;
                    existingStudent.AlternateMobileNumber = studentProfile.AlternateMobileNumber;
                    existingStudent.EmailAddress = studentProfile.EmailAddress;
                    existingStudent.ProfilePicture = studentProfile.ProfilePicture;
                    existingStudent.ModifiedBy = currentUser.Name;
                    existingStudent.ModifiedOn = DateTime.Now;

                    if (!isStudentExist)
                        dbContext.Students.Add(existingStudent);
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
