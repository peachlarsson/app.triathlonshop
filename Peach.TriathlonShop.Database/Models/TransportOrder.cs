using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.Database.Models;

public class TransportOrder : BaseEntity
{
    public required long OrderId { get; set; }
    public required TransportStatus Status { get; set; }
    public string? ConsignmentNumber { get; set; }
    public string? TrackingUrl { get; set; }
    public string? Carrier { get; set; }
    public string? Destination { get; set; }
    public DateTimeOffset? ShippedAt { get; set; }
    public DateTimeOffset? EstimatedDeliveryDate { get; set; }

    public Order? Order { get; set; } = null!;
}
