using HealthPlus.API.DTOs.Order;

namespace HealthPlus.API.Services.Interfaces;

public interface IOrderService
{
    Task<(bool Success, string Message, OrderDto? Data)>
        CreateAsync(int userId, CreateOrderRequest request);

    Task<List<OrderDto>> GetMyOrdersAsync(int userId);

    Task<(bool Success, string Message, OrderDto? Data)>
        GetByIdAsync(int userId, int orderId);
}