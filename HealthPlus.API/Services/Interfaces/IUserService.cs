using HealthPlus.API.DTOs.User;

namespace HealthPlus.API.Services.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();

    Task<(bool Success, string Message, UserDto? Data)>
    UpdateStatusAsync(
        int currentUserId,
        int userId,
        bool isActive);
}