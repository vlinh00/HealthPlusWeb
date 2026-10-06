using HealthPlus.API.DTOs.Payment;

namespace HealthPlus.API.Services.Interfaces;

public interface IPaymentService
{
    Task<(bool Success, string Message, PaymentDto? Data)>
        ProcessPaymentAsync(
            int userId,
            int orderId,
            PaymentRequest request);
}