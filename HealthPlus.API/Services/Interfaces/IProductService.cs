using HealthPlus.API.Common;
using HealthPlus.API.DTOs.Product;

namespace HealthPlus.API.Services.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetProductsAsync(
        string? search,
        int? categoryId,
        string? sort,
        int page,
        int pageSize);

    Task<ProductDto?> GetByIdAsync(int id);

    Task<ProductDto> CreateAsync(ProductRequest request);

    Task<bool> UpdateAsync(int id, ProductRequest request);

    Task<bool> DeleteAsync(int id);
}