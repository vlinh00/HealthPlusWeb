using System.ComponentModel.DataAnnotations;

namespace HealthPlus.API.DTOs.Payment;

public class PaymentRequest
{
    [Required]
    public string Method { get; set; } = "Mock";
}