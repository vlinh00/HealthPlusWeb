using HealthPlus.API.Data;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthPlus.API.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly HealthPlusDbContext _context;

    public UserRepository(HealthPlusDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task UpdateAsync(User user)
    {
        await _context.SaveChangesAsync();
    }
}