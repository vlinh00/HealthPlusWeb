using HealthPlus.API.Entities;

namespace HealthPlus.API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(int id);

    Task<User> AddAsync(User user);

    Task<List<User>> GetAllAsync();

    Task UpdateAsync(User user);
}