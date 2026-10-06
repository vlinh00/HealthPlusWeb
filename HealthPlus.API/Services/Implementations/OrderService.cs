using HealthPlus.API.Data;
using HealthPlus.API.DTOs.Order;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using HealthPlus.API.Services.Interfaces;

namespace HealthPlus.API.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly HealthPlusDbContext _context;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        HealthPlusDbContext context)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _context = context;
    }

    public async Task<(bool Success, string Message, OrderDto? Data)>
        CreateAsync(int userId, CreateOrderRequest request)
    {
        var cartItems =
            await _cartRepository.GetByUserIdForUpdateAsync(userId);

        if (cartItems.Count == 0)
        {
            return (false, "Cart is empty.", null);
        }

        // Validate cart and stock
        foreach (var cartItem in cartItems)
        {
            if (cartItem.Product == null ||
                !cartItem.Product.IsActive)
            {
                return (
                    false,
                    $"Product ID {cartItem.ProductId} is no longer available.",
                    null);
            }

            if (cartItem.Quantity > cartItem.Product.Stock)
            {
                return (
                    false,
                    $"Product '{cartItem.Product.Name}' does not have enough stock.",
                    null);
            }
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            decimal totalAmount = 0;

            foreach (var cartItem in cartItems)
            {
                totalAmount +=
                    cartItem.Product.Price *
                    cartItem.Quantity;
            }

            // Generate OrderCode
            var orderCode =
                $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

            var order = new Order
            {
                OrderCode = orderCode,

                UserId = userId,

                OrderDate = DateTime.UtcNow,

                TotalAmount = totalAmount,

                Status = "Pending",

                PaymentStatus = "Pending",

                ShippingAddress =
                    request.ShippingAddress.Trim(),

                Phone =
                    request.Phone.Trim(),

                CreatedAt = DateTime.UtcNow
            };

            // Create OrderItems
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,

                    ProductName =
                        cartItem.Product.Name,

                    UnitPrice =
                        cartItem.Product.Price,

                    Quantity =
                        cartItem.Quantity
                };

                order.OrderItems.Add(orderItem);

                // Decrease stock
                cartItem.Product.Stock -=
                    cartItem.Quantity;
            }

            _context.Orders.Add(order);

            // Clear cart after creating order
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

    public async Task<List<OrderDto>>
        GetMyOrdersAsync(int userId)
    {
        var orders =
            await _orderRepository.GetByUserIdAsync(userId);

        return orders
            .Select(MapToDto)
            .ToList();
    }

    public async Task<(bool Success, string Message, OrderDto? Data)>
        GetByIdAsync(int userId, int orderId)
    {
        var order =
            await _orderRepository.GetByIdAsync(
                userId,
                orderId);

        if (order == null)
        {
            return (
                false,
                "Order not found.",
                null);
        }

        return (
            true,
            "Success.",
            MapToDto(order));
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,

            OrderDate = order.OrderDate,

            Status = order.Status,

            TotalAmount = order.TotalAmount,

            ShippingAddress =
                order.ShippingAddress,

            Phone = order.Phone,

            Items = order.OrderItems
                .Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,

                    ProductName =
                        item.ProductName,

                    UnitPrice =
                        item.UnitPrice,

                    Quantity =
                        item.Quantity
                })
                .ToList()
        };
    }
}