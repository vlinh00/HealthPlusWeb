using HealthPlus.API.Common;
using HealthPlus.API.DTOs.Auth;
using HealthPlus.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlus.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        var result =
            await _authService.RegisterAsync(request);

        if (!result.Success)
        {
            return BadRequest(
                ApiResponse<object>.Fail(
                    result.Message));
        }

        return Ok(
            new ApiResponse<object>
            {
                Success = true,
                Message = result.Message,
                Data = result.Data
            });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        var result =
            await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(
                ApiResponse<object>.Fail(
                    result.Message));
        }

        return Ok(
            ApiResponse<LoginResponse>.Ok(
                result.Data!,
                result.Message));
    }
}