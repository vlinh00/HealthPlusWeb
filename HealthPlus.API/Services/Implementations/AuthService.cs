using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HealthPlus.API.DTOs.Auth;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using HealthPlus.API.Services.Interfaces;
using HealthPlus.API.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HealthPlus.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;

        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<(bool Success, string Message, object? Data)> RegisterAsync(
        RegisterRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var existingUser =
            await _userRepository.GetByEmailAsync(email);

        if (existingUser != null)
        {
            return (
                false,
                "Email already exists.",
                null);
        }

        var user = new User
        {
            Email = email,

            FullName = request.FullName.Trim(),

            Phone = request.Phone?.Trim(),

            Address = request.Address?.Trim(),

            Role = "Customer",

            IsActive = true,

            CreatedAt = DateTime.UtcNow,

            UpdatedAt = DateTime.UtcNow
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(
                user,
                request.Password);

        await _userRepository.AddAsync(user);

        return (
            true,
            "Registration successful.",
            new
            {
                user.Id,
                user.Email,
                user.FullName,
                user.Role
            });
    }

    public async Task<(bool Success, string Message, LoginResponse? Data)> LoginAsync(
        LoginRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var user =
            await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            return (
                false,
                "Invalid email or password.",
                null);
        }

        if (!user.IsActive)
        {
            return (
                false,
                "Your account has been disabled.",
                null);
        }

        var passwordResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return (
                false,
                "Invalid email or password.",
                null);
        }

        var expiresAt =
            DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpireMinutes);

        var token =
            GenerateToken(user, expiresAt);

        return (
            true,
            "Login successful.",
            new LoginResponse
            {
                Token = token,

                UserId = user.Id,

                Email = user.Email,

                FullName = user.FullName,

                Role = user.Role,

                ExpiresAt = expiresAt
            });
    }

    private string GenerateToken(
        User user,
        DateTime expiresAt)
    {
        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new(
                ClaimTypes.Email,
                user.Email),

            new(
                ClaimTypes.Name,
                user.FullName),

            new(
                ClaimTypes.Role,
                user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _jwtSettings.Key));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}