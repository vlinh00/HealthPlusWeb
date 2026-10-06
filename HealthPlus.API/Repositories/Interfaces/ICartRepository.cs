using HealthPlus.API.Entities;

namespace HealthPlus.API.Repositories.Interfaces;

public interface ICartRepository
{
    Task<List<CartItem>> GetByUserIdAsync(int userId);

    Task<CartItem?> GetItemAsync(int userId, int productId);

    Task<CartItem> AddAsync(CartItem cartItem);

    Task UpdateAsync(CartItem cartItem);

    Task DeleteAsync(CartItem cartItem);
}