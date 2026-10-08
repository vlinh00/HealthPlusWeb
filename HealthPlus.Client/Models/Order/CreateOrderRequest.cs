namespace HealthPlus.Client.Models.Order;

public class CreateOrderRequest
{
    public string ShippingAddress { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
}