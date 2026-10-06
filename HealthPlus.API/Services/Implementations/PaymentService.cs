using HealthPlus.API.Data;
using HealthPlus.API.DTOs.Payment;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using HealthPlus.API.Services.Interfaces;

namespace HealthPlus.API.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly HealthPlusDbContext _context;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        HealthPlusDbContext context)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _context = context;
    }

    public async Task<(bool Success, string Message, PaymentDto? Data)>
        ProcessPaymentAsync(
            int userId,
            int orderId,
            PaymentRequest request)
    {
        // Validate payment method
        var method = request.Method.Trim();

        if (method != "COD" && method != "MockCard")
        {
            return (
                false,
                "Invalid payment method. Supported methods: COD, MockCard.",
                null);
        }

        // Get order for update
        var order =
            await _orderRepository.GetByIdForUpdateAsync(
                userId,
                orderId);

        if (order == null)
        {
            return (
                false,
                "Order not found.",
                null);
        }

        // Cannot pay cancelled order
        if (order.Status == "Cancelled")
        {
            return (
                false,
                "Cannot pay for a cancelled order.",
                null);
        }

        // Already paid
        if (order.PaymentStatus == "Paid")
        {
            return (
                false,
                "Order has already been paid.",
                null);
        }

        // Check existing payment
        var existingPayment =
            await _paymentRepository.GetByOrderIdAsync(orderId);

        if (existingPayment != null &&
            existingPayment.Status == "Paid")
        {
            return (
                false,
                "Order has already been paid.",
                null);
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // MockCard:
            // Payment is completed immediately.
            //
            // COD:
            // Payment remains pending until payment is collected.
            var isMockCard = method == "MockCard";

            var paymentStatus = isMockCard
                ? "Paid"
                : "Pending";

            var transactionCode = isMockCard
                ? $"MOCK-{Guid.NewGuid().ToString("N")[..12].ToUpper()}"
                : null;

            var payment = new Payment
            {
                OrderId = order.Id,

                Method = method,

                Amount = order.TotalAmount,

                Status = paymentStatus,

                TransactionCode = transactionCode,

                PaymentDate = isMockCard
                    ? DateTime.UtcNow
                    : null
            };

            _context.Payments.Add(payment);

            // Update order payment status
            order.PaymentStatus = paymentStatus;

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var result = new PaymentDto
            {
                Id = payment.Id,

                OrderId = payment.OrderId,

                Method = payment.Method,

                Amount = payment.Amount,

                Status = payment.Status,

                TransactionCode = payment.TransactionCode,

                PaymentDate = payment.PaymentDate
            };

            var message = isMockCard
                ? "Payment successful."
                : "Order placed with COD successfully.";

            return (
                true,
                message,
                result);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}