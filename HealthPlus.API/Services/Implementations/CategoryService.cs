using HealthPlus.API.DTOs.Category;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using HealthPlus.API.Services.Interfaces;

namespace HealthPlus.API.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();

        return categories
            .Select(MapToDto)
            .ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category == null)
            return null;

        return MapToDto(category);
    }

    public async Task<(CategoryDto? Data, string? Error)> CreateAsync(
    CategoryRequest request)
{
    var name = request.Name.Trim();

    if (await _repository.ExistsByNameAsync(name))
    {
        return (null, "Tên danh mục đã tồn tại.");
    }

    var category = new Category
    {
        Name = name,
        Description = string.IsNullOrWhiteSpace(request.Description)
            ? null
            : request.Description.Trim()
    };

    await _repository.AddAsync(category);

    return (MapToDto(category), null);
}

public async Task<(CategoryDto? Data, string? Error)> UpdateAsync(
    int id,
    CategoryRequest request)
{
    var category = await _repository.GetByIdAsync(id);

    if (category == null)
    {
        return (null, "Không tìm thấy danh mục.");
    }

    var name = request.Name.Trim();

    if (await _repository.ExistsByNameAsync(name, id))
    {
        return (null, "Tên danh mục đã được sử dụng bởi danh mục khác.");
    }

    category.Name = name;
    category.Description = string.IsNullOrWhiteSpace(request.Description)
        ? null
        : request.Description.Trim();

    await _repository.UpdateAsync(category);

    return (MapToDto(category), null);
}
    public async Task<(bool Success, string Message)> DeleteAsync(
        int id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category == null)
        {
            return (
                false,
                "Không tìm thấy danh mục.");
        }

        var hasProducts =
            await _repository.HasProductsAsync(id);

        if (hasProducts)
        {
            return (
                false,
                "Không thể xóa danh mục đang có sản phẩm.");
        }

        await _repository.DeleteAsync(category);

        return (
            true,
            "Xóa danh mục thành công.");
    }

    private static CategoryDto MapToDto(
        Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}