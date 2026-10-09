using System.ComponentModel.DataAnnotations;

namespace HealthPlus.API.DTOs.Cart;

public class AddCartItemRequest
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}