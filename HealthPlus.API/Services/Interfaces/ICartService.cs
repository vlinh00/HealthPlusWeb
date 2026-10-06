using HealthPlus.API.DTOs.Cart;

namespace HealthPlus.API.Services.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(int userId);

    Task<(bool Success, string Message, CartDto? Data)>
        AddItemAsync(int userId, AddCartItemRequest request);

    Task<(bool Success, string Message, CartDto? Data)>
        UpdateItemAsync(int userId, int productId, UpdateCartItemRequest request);

    Task<(bool Success, string Message)>
        RemoveItemAsync(int userId, int productId);
}