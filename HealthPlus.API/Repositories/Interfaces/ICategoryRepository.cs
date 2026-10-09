using HealthPlus.API.Entities;

namespace HealthPlus.API.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();

    Task<Category?> GetByIdAsync(int id);

    Task<Category> AddAsync(Category category);

    Task UpdateAsync(Category category);

    Task DeleteAsync(Category category);

    Task<bool> HasProductsAsync(int categoryId);

    Task<bool> ExistsByNameAsync(
    string name,
    int? excludeId = null);
}