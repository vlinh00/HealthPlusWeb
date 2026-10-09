using HealthPlus.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlus.API.Repositories.Interfaces;

public interface IProductRepository
{
    Task<(List<Product> Items, int TotalItems)> GetPagedAsync(
        string? search,
        int? categoryId,
        string? sort,
        int page,
        int pageSize);

    Task<Product?> GetByIdAsync(int id);

    Task<Product> AddAsync(Product product);

    Task UpdateAsync(Product product);
}