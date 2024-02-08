using ResoClassAPI.DTOs;
namespace ResoClassAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsers();
        Task<UserDto> GetUser(int userId);
        Task<UserDto> GetUserWithUserName(string userName);
        Task<bool> CreateUser(UserDto newItem);
        Task<bool> UpdateUser(UserDto updatedItem);
        Task<bool> ChangePassword(int id, string password);
        Task<bool> DeleteUser(int userId);
    }
}
