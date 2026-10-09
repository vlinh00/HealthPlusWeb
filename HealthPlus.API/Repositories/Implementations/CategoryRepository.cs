using HealthPlus.API.Data;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthPlus.API.Repositories.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly HealthPlusDbContext _context;

    public CategoryRepository(HealthPlusDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Category> AddAsync(Category category)
    {
        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return category;
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasProductsAsync(int categoryId)
    {
        return await _context.Products
            .AnyAsync(x => x.CategoryId == categoryId);
    }

    public async Task<bool> ExistsByNameAsync(
    string name,
    int? excludeId = null)
{
    var query = _context.Categories
        .AsNoTracking()
        .Where(x => x.Name == name);

    if (excludeId.HasValue)
    {
        query = query.Where(x => x.Id != excludeId.Value);
    }

    return await query.AnyAsync();
}
}