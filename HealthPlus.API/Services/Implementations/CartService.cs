using HealthPlus.API.DTOs.Cart;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using HealthPlus.API.Services.Interfaces;

namespace HealthPlus.API.Services.Implementations;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto> GetCartAsync(int userId)
    {
        var items = await _cartRepository.GetByUserIdAsync(userId);

        return MapToDto(items);
    }

    public async Task<(bool Success, string Message, CartDto? Data)>
        AddItemAsync(int userId, AddCartItemRequest request)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
            return (false, "Product not found.", null);

        if (product.Stock <= 0)
            return (false, "Product is out of stock.", null);

        if (request.Quantity > product.Stock)
            return (false, "Requested quantity exceeds stock.", null);

        var existingItem = await _cartRepository
            .GetItemAsync(userId, request.ProductId);

        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + request.Quantity;

            if (newQuantity > product.Stock)
                return (false, "Requested quantity exceeds stock.", null);

            existingItem.Quantity = newQuantity;

            await _cartRepository.UpdateAsync(existingItem);
        }
        else
        {
            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };

            await _cartRepository.AddAsync(cartItem);
        }

        var cart = await GetCartAsync(userId);

        return (true, "Product added to cart.", cart);
    }

    public async Task<(bool Success, string Message, CartDto? Data)>
        UpdateItemAsync(
            int userId,
            int productId,
            UpdateCartItemRequest request)
    {
        var item = await _cartRepository
            .GetItemAsync(userId, productId);

        if (item == null)
            return (false, "Cart item not found.", null);

        if (item.Product == null)
            return (false, "Product not found.", null);

        if (request.Quantity > item.Product.Stock)
            return (false, "Requested quantity exceeds stock.", null);

        item.Quantity = request.Quantity;

        await _cartRepository.UpdateAsync(item);

        var cart = await GetCartAsync(userId);

        return (true, "Cart updated.", cart);
    }

    public async Task<(bool Success, string Message)>
        RemoveItemAsync(int userId, int productId)
    {
        var item = await _cartRepository
            .GetItemAsync(userId, productId);

        if (item == null)
            return (false, "Cart item not found.");

        await _cartRepository.DeleteAsync(item);

        return (true, "Product removed from cart.");
    }

    private static CartDto MapToDto(List<CartItem> items)
{
    var cartItems = items.Select(x => new CartItemDto
    {
        ProductId = x.ProductId,
        ProductName = x.Product.Name,
        Price = x.Product.Price,
        Quantity = x.Quantity,
        ImageUrl = x.Product.ImageUrl
    }).ToList();

    return new CartDto
    {
        Items = cartItems,
        TotalAmount = cartItems.Sum(x => x.Subtotal)
    };
}
}