using HealthPlus.API.DTOs.Auth;

namespace HealthPlus.API.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Message, object? Data)> RegisterAsync(
        RegisterRequest request);

    Task<(bool Success, string Message, LoginResponse? Data)> LoginAsync(
        LoginRequest request);
}