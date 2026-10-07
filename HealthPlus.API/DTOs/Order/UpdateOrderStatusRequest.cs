using System.ComponentModel.DataAnnotations;

namespace HealthPlus.API.DTOs.Order;

public class UpdateOrderStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;
}