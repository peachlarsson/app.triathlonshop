using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.App.Server.Controllers.Models;

public class OrderDto
{
    public required long Id { get; set; }
    public required OrderStatus Status { get; set; }
    public required string Number { get; set; }
    public List<OrderRowDto> Items { get; set; } = [];
    public List<NotificationHistoryDto> NotificationHistory { get; set; } = [];
    public List<TransportHistoryDto> TransportHistory { get; set; } = [];
    public List<InvoiceHistoryDto> InvoiceHistory { get; set; } = [];
    public required decimal TotalPrice { get; set; }
    public int OrderRowsCount => Items.Count;
    public required long CustomerId { get; set; }
    public required string CustomerName { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class NotificationHistoryDto
{
    public required long Id { get; set; }
    public required string OrderStatus { get; set; }
    public required string Destination { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class TransportHistoryDto
{
    public required long Id { get; set; }
    public required string Status { get; set; }
    public string? Carrier { get; set; }
    public string? ConsignmentNumber { get; set; }
    public string? TrackingUrl { get; set; }
    public string? Destination { get; set; }
    public DateTimeOffset? ShippedAt { get; set; }
    public DateTimeOffset? EstimatedDeliveryDate { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class InvoiceHistoryDto
{
    public required long Id { get; set; }
    public required string Status { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
