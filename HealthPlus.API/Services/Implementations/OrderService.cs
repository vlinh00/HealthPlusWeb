using HealthPlus.API.DTOs.Order;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using HealthPlus.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using HealthPlus.API.Data;

namespace HealthPlus.API.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly HealthPlusDbContext _context;
    private readonly ICartRepository _cartRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderService(
        HealthPlusDbContext context,
        ICartRepository cartRepository,
        IOrderRepository orderRepository)
    {
        _context = context;
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
    }

    public async Task<(bool Success, string Message, OrderDto? Data)>
        CreateAsync(
            int userId,
            CreateOrderRequest request)
    {
        var cartItems = await _cartRepository.GetByUserIdAsync(userId);

        if (cartItems.Count == 0)
            return (false, "Your cart is empty.", null);

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // Reload products as tracked entities
            var productIds = cartItems
                .Select(x => x.ProductId)
                .Distinct()
                .ToList();

            var products = await _context.Products
                .Where(x => productIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);

            // Validate stock
            foreach (var cartItem in cartItems)
            {
                if (!products.TryGetValue(
                        cartItem.ProductId,
                        out var product))
                {
                    await transaction.RollbackAsync();

                    return (
                        false,
                        $"Product {cartItem.ProductId} not found.",
                        null);
                }

                if (!product.IsActive)
                {
                    await transaction.RollbackAsync();

                    return (
                        false,
                        $"Product '{product.Name}' is no longer available.",
                        null);
                }

                if (cartItem.Quantity > product.Stock)
                {
                    await transaction.RollbackAsync();

                    return (
                        false,
                        $"Product '{product.Name}' does not have enough stock.",
                        null);
                }
            }

            // Calculate total
            var totalAmount = cartItems.Sum(x =>
                products[x.ProductId].Price * x.Quantity);

            // Create Order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = totalAmount,
                ShippingAddress = request.ShippingAddress.Trim(),
                Phone = request.Phone.Trim()
            };

            foreach (var cartItem in cartItems)
            {
                var product = products[cartItem.ProductId];

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = cartItem.Quantity
                });

                // Reduce stock
                product.Stock -= cartItem.Quantity;
            }

            _context.Orders.Add(order);

            // Clear cart
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return (
                true,
                "Order created successfully.",
                MapToDto(order));
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }

    public async Task<List<OrderDto>> GetMyOrdersAsync(int userId)
    {
        var orders = await _orderRepository
            .GetByUserIdAsync(userId);

        return orders
            .Select(MapToDto)
            .ToList();
    }

    public async Task<(bool Success, string Message, OrderDto? Data)>
        GetByIdAsync(int userId, int orderId)
    {
        var order = await _orderRepository
            .GetByIdAsync(userId, orderId);

        if (order == null)
            return (false, "Order not found.", null);

        return (true, "Success.", MapToDto(order));
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            ShippingAddress = order.ShippingAddress,
            Phone = order.Phone,

            Items = order.OrderItems
                .Select(x => new OrderItemDto
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    UnitPrice = x.UnitPrice,
                    Quantity = x.Quantity
                })
                .ToList()
        };
    }
}