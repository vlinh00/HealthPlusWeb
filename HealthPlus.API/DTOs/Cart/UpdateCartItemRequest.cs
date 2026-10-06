using System.ComponentModel.DataAnnotations;

namespace HealthPlus.API.DTOs.Cart;

public class UpdateCartItemRequest
{
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}