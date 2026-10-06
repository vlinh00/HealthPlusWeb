namespace HealthPlus.API.DTOs.Payment;

public class PaymentDto
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string Method { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? TransactionCode { get; set; }

    public DateTime? PaymentDate { get; set; }
}