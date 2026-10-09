using HealthPlus.API.Data;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthPlus.API.Repositories.Implementations;

public class CartRepository : ICartRepository
{
    private readonly HealthPlusDbContext _context;

    public CartRepository(HealthPlusDbContext context)
    {
        _context = context;
    }

    public async Task<List<CartItem>> GetByUserIdAsync(int userId)
    {
        return await _context.CartItems
            .AsNoTracking()
            .Include(x => x.Product)
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<List<CartItem>> GetByUserIdForUpdateAsync(int userId)
{
    return await _context.CartItems
        .Include(x => x.Product)
        .Where(x => x.UserId == userId)
        .OrderBy(x => x.Id)
        .ToListAsync();
}

    public async Task<CartItem?> GetItemAsync(int userId, int productId)
    {
        return await _context.CartItems
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.ProductId == productId);
    }

    public async Task<CartItem> AddAsync(CartItem cartItem)
    {
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();

        return cartItem;
    }

    public async Task UpdateAsync(CartItem cartItem)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
    }

    
}