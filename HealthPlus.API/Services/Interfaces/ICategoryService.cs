using HealthPlus.API.DTOs.Category;

namespace HealthPlus.API.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();

    Task<CategoryDto?> GetByIdAsync(int id);

    Task<(CategoryDto? Data, string? Error)> CreateAsync(
    CategoryRequest request);

Task<(CategoryDto? Data, string? Error)> UpdateAsync(
    int id,
    CategoryRequest request);

    Task<(bool Success, string Message)> DeleteAsync(int id);

}