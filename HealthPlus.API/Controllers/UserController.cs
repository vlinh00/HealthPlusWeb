using HealthPlus.API.Common;
using HealthPlus.API.DTOs.User;
using HealthPlus.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthPlus.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users =
            await _userService.GetAllAsync();

        return Ok(
            ApiResponse<List<UserDto>>.Ok(
                users));
    }

    [HttpPut("{userId:int}/status")]
public async Task<IActionResult> UpdateStatus(
    int userId,
    UpdateUserStatusRequest request)
{
    var currentUserIdClaim =
        User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (!int.TryParse(currentUserIdClaim, out var currentUserId))
    {
        return Unauthorized();
    }

    var result =
        await _userService.UpdateStatusAsync(
            currentUserId,
            userId,
            request.IsActive);

    if (!result.Success)
    {
        return BadRequest(
            ApiResponse<UserDto?>.Fail(
                result.Message));
    }

    return Ok(
        ApiResponse<UserDto?>.Ok(
            result.Data!,
            result.Message));
}
}