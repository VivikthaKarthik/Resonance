using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using System.Collections.Generic;

namespace ResoClassAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        public UserService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = await Task.FromResult(dbContext.Users.Where(item => item.IsActive == true).OrderByDescending(x => x.CreatedOn).ToList());
            if (users != null && users.Count > 0)
            {
                List<UserDto> dtoObjects = new List<UserDto>();     
                foreach (var user in users)
                {
                    var dtoObject = mapper.Map<UserDto>(user);
                    dtoObject.Role = dbContext.Roles.First(item => item.Id == user.RoleId).Name;
                    dtoObjects.Add(dtoObject);
                }
                return dtoObjects;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<UserDto> GetUser(int userId)
        {
            var user = await Task.FromResult(dbContext.Users.FirstOrDefault(item => item.Id == userId && item.IsActive == true));
            if (user != null)
            {
                var dtoObject = mapper.Map<UserDto>(user);
                dtoObject.Role = dbContext.Roles.First(item => item.Id == user.RoleId).Name;
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<UserDto> GetUserWithUserName(string userName)
        {
            var user = await Task.FromResult(dbContext.Users.FirstOrDefault(item => (item.Email.ToLower() == userName.ToLower() || item.PhoneNumber.ToLower() == userName.ToLower()) && item.IsActive == true));
            if (user != null)
            {
                var dtoObject = mapper.Map<UserDto>(user);
                dtoObject.Role = dbContext.Roles.First(item => item.Id == user.RoleId).Name;
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<bool> CreateUser(UserDto user)
        {
            var currentUser = authService.GetCurrentUser();

            if (currentUser != null)
            {
                var newUser = mapper.Map<User>(user);
                if (!string.IsNullOrEmpty(user.Role))
                {
                    var role = dbContext.Roles.FirstOrDefault(x => x.Name.ToLower() == user.Role.ToLower());
                    if (role != null)
                        newUser.RoleId = role.Id;
                    else
                        throw new Exception("Role does not exist");
                }
                else
                    throw new Exception("Invalid Role");

                newUser.CreatedBy = newUser.ModifiedBy = currentUser.Name;
                newUser.CreatedOn = newUser.ModifiedOn = DateTime.Now;
                newUser.IsActive = true;
                dbContext.Users.Add(newUser);
                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateUser(UserDto updatedUser)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Users.FirstOrDefault(item => item.Id == updatedUser.Id && item.IsActive == true);

            if (existingItem != null)
            {
                if (!string.IsNullOrEmpty(updatedUser.Role))
                {
                    var role = dbContext.Roles.FirstOrDefault(x => x.Name.ToLower() == updatedUser.Role.ToLower());

                    if (role != null)
                        existingItem.RoleId = role.Id;
                    else
                        throw new Exception("Updated Role does not exist");
                }

                existingItem.FirstName = updatedUser.FirstName;
                existingItem.LastName = updatedUser.LastName;
                existingItem.ModifiedBy = currentUser.Name;
                existingItem.ModifiedOn = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ChangePassword(int id, string password)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Users.FirstOrDefault(item => item.Id == id && item.IsActive == true);

            if (existingItem != null)
            {
                existingItem.Password = password;
                existingItem.ModifiedBy = currentUser.Name;
                existingItem.ModifiedOn = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Users.FirstOrDefault(item => item.Id == userId);

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
