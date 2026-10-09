using HealthPlus.API.Data;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthPlus.API.Repositories.Implementations;

public class ProductRepository : IProductRepository
{
    private readonly HealthPlusDbContext _context;

    public ProductRepository(HealthPlusDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Product> Items, int TotalItems)> GetPagedAsync(
        string? search,
        int? categoryId,
        string? sort,
        int page,
        int pageSize)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.IsActive)
            .AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.Name.Contains(search) ||
                (x.Description != null &&
                 x.Description.Contains(search)));
        }

        // Category
        if (categoryId.HasValue)
        {
            query = query.Where(x =>
                x.CategoryId == categoryId.Value);
        }

        // Total before pagination
        var totalItems = await query.CountAsync();

        // Sort
        query = sort?.Trim().ToLowerInvariant() switch
        {
            "price_asc" =>
                query
                    .OrderBy(x => x.Price)
                    .ThenBy(x => x.Id),

            "price_desc" =>
                query
                    .OrderByDescending(x => x.Price)
                    .ThenBy(x => x.Id),

            "name_asc" =>
                query
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Id),

            _ =>
                query.OrderByDescending(x => x.Id)
        };

        // Pagination
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.IsActive);
    }

    public async Task<Product> AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}