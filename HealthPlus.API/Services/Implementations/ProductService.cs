using HealthPlus.API.Common;
using HealthPlus.API.DTOs.Product;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using HealthPlus.API.Services.Interfaces;

namespace HealthPlus.API.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(
        string? search,
        int? categoryId,
        int page,
        int pageSize)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 10;

        if (pageSize > 100)
            pageSize = 100;

        var result = await _productRepository.GetPagedAsync(
            search,
            categoryId,
            page,
            pageSize);

        return new PagedResult<ProductDto>
        {
            Items = result.Items
                .Select(MapToDto)
                .ToList(),

            Page = page,

            PageSize = pageSize,

            TotalItems = result.TotalItems
        };
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return null;

        return MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(ProductRequest request)
    {
        var product = new Product
        {
            CategoryId = request.CategoryId,

            Name = request.Name.Trim(),

            Description = request.Description,

            Price = request.Price,

            Stock = request.Stock,

            ImageUrl = request.ImageUrl,

            IsActive = request.IsActive,

            CreatedAt = DateTime.UtcNow,

            UpdatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product);

        // EF không tự load Category sau khi insert
        // nên lấy lại entity để có CategoryName.
        var createdProduct =
            await _productRepository.GetByIdAsync(product.Id);

        return MapToDto(createdProduct!);
    }

    public async Task<bool> UpdateAsync(
        int id,
        ProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return false;

        product.CategoryId = request.CategoryId;

        product.Name = request.Name.Trim();

        product.Description = request.Description;

        product.Price = request.Price;

        product.Stock = request.Stock;

        product.ImageUrl = request.ImageUrl;

        product.IsActive = request.IsActive;

        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return false;

        // Soft delete
        product.IsActive = false;

        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);

        return true;
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,

            CategoryId = product.CategoryId,

            CategoryName = product.Category?.Name ?? string.Empty,

            Name = product.Name,

            Description = product.Description,

            Price = product.Price,

            Stock = product.Stock,

            ImageUrl = product.ImageUrl,

            IsActive = product.IsActive
        };
    }
}