using HealthPlus.API.Data;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthPlus.API.Repositories.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly HealthPlusDbContext _context;

    public OrderRepository(HealthPlusDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateAsync(Order order)
    {
        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<List<Order>> GetByUserIdAsync(int userId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(x => x.OrderItems)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int userId, int orderId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x =>
                x.Id == orderId &&
                x.UserId == userId);
    }
}