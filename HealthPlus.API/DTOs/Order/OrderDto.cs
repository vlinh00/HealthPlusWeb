namespace HealthPlus.API.DTOs.Order;

public class OrderDto
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;

    public string PaymentStatus { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public List<OrderItemDto> Items { get; set; } = [];
}