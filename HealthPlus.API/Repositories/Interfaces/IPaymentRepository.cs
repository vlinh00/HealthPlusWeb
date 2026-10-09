using HealthPlus.API.Entities;

namespace HealthPlus.API.Repositories.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByOrderIdAsync(int orderId);

    Task<Payment> AddAsync(Payment payment);
}