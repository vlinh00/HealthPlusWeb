using HealthPlus.API.Common;
using HealthPlus.API.DTOs.Category;
using HealthPlus.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlus.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }

    // Customer có thể lấy danh sách category
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();

        return Ok(
            ApiResponse<List<CategoryDto>>.Ok(result));
    }

    // Customer có thể xem category
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
        {
            return NotFound(
                ApiResponse<CategoryDto>.Fail(
                    "Không tìm thấy danh mục."));
        }

        return Ok(
            ApiResponse<CategoryDto>.Ok(result));
    }

    // Admin tạo category
    [HttpPost]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Create(
    [FromBody] CategoryRequest request)
{
    var result = await _service.CreateAsync(request);

    if (result.Error != null)
    {
        return Conflict(
            ApiResponse<CategoryDto>.Fail(result.Error));
    }

    return Ok(
        ApiResponse<CategoryDto>.Ok(
            result.Data!,
            "Tạo danh mục thành công."));
}

[HttpPut("{id:int}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Update(
    int id,
    [FromBody] CategoryRequest request)
{
    var result = await _service.UpdateAsync(id, request);

    if (result.Error == "Không tìm thấy danh mục.")
    {
        return NotFound(
            ApiResponse<CategoryDto>.Fail(result.Error));
    }

    if (result.Error != null)
    {
        return Conflict(
            ApiResponse<CategoryDto>.Fail(result.Error));
    }

    return Ok(
        ApiResponse<CategoryDto>.Ok(
            result.Data!,
            "Cập nhật danh mục thành công."));
}

    // Admin xóa category
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result =
            await _service.DeleteAsync(id);

        if (!result.Success)
        {
            return BadRequest(
                ApiResponse<object>.Fail(
                    result.Message));
        }

        return Ok(
            ApiResponse<object>.Ok(
                null!,
                result.Message));
    }
}