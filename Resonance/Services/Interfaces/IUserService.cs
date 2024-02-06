using ResoClassAPI.DTOs;
namespace ResoClassAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUser(int userId);
        Task<bool> CreateUser(UserDto newItem);
        Task<bool> UpdateUser(UserDto updatedItem);
        Task<bool> DeleteUser(int userId);
    }
}
