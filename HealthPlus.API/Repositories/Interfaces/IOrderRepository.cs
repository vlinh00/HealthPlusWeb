using HealthPlus.API.Entities;

namespace HealthPlus.API.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);

    Task<List<Order>> GetByUserIdAsync(int userId);

    Task<Order?> GetByIdAsync(int userId, int orderId);

    Task<Order?> GetByIdForUpdateAsync(int userId, int orderId);

    Task<List<Order>> GetAllAsync();
}