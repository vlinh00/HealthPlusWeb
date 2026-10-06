namespace HealthPlus.API.DTOs.Cart;

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
}

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => Price * Quantity;
    public string? ImageUrl { get; set; }
}