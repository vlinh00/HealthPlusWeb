using System.ComponentModel.DataAnnotations;

namespace HealthPlus.API.DTOs.Order;

public class CreateOrderRequest
{
    [Required]
    [MaxLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
}