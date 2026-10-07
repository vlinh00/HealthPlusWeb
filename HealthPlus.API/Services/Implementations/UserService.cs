using HealthPlus.API.DTOs.User;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using HealthPlus.API.Services.Interfaces;

namespace HealthPlus.API.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users =
            await _userRepository.GetAllAsync();

        return users
            .Select(MapToDto)
            .ToList();
    }

    public async Task<(bool Success, string Message, UserDto? Data)>
    UpdateStatusAsync(
        int currentUserId,
        int userId,
        bool isActive)
    {
        // Admin cannot lock/unlock their own account
        if (currentUserId == userId)
        {
            return (
                false,
                "You cannot change your own account status.",
                null);
        }

        var user =
            await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return (
                false,
                "User not found.",
                null);
        }

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return (
            true,
            isActive
                ? "User activated successfully."
                : "User blocked successfully.",
            MapToDto(user));
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            Address = user.Address,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}