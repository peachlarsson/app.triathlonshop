namespace Peach.TriathlonShop.Api.Contract.Models.Orders;

public class TransportOrderDto
{
    public required long Id { get; set; }
    public string? ConsignmentNumber { get; set; }
    public string? TransportOrderStatus { get; set; }
    public string? Carrier { get; set; }
    public string? Destination { get; set; }
    public DateTimeOffset? DispatchDateTime { get; set; }
    public DateTimeOffset? ShippedAt { get; set; }
    public DateTimeOffset? EstimatedDeliveryDate { get; set; }
}
