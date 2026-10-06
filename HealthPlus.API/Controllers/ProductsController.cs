using HealthPlus.API.Common;
using HealthPlus.API.DTOs.Product;
using HealthPlus.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlus.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/products
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _productService.GetProductsAsync(
            search,
            categoryId,
            page,
            pageSize);

        return Ok(ApiResponse<PagedResult<ProductDto>>.Ok(result));
    }

    // GET: api/products/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound(
                ApiResponse<ProductDto>.Fail(
                    "Product not found."));
        }

        return Ok(
            ApiResponse<ProductDto>.Ok(product));
    }

    // POST: api/products
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromBody] ProductRequest request)
    {
        var product = await _productService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            ApiResponse<ProductDto>.Ok(
                product,
                "Product created successfully."));
    }

    // PUT: api/products/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(
        int id,
        [FromBody] ProductRequest request)
    {
        var updated = await _productService.UpdateAsync(
            id,
            request);

        if (!updated)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Product not found."));
        }

        var product = await _productService.GetByIdAsync(id);

        return Ok(
            ApiResponse<ProductDto>.Ok(
                product!,
                "Product updated successfully."));
    }

    // DELETE: api/products/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deleted = await _productService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Product not found."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                null!,
                "Product deleted successfully."));
    }
}