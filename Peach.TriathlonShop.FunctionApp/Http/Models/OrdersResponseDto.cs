using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.FunctionApp.Http.Models;

public class OrderResponseDto
{
    public required long Id { get; set; }
    public required OrderStatus Status { get; set; }
    public required string Number { get; set; }
    public required OrderCustomerDto Customer { get; set; }
    public List<OrderRowDto> OrderRows { get; set; } = [];
    public List<OrderNotificationDto> Notifications { get; set; } = [];
    public List<OrderTransportEventDto> TransportEvents { get; set; } = [];
    public List<OrderInvoiceDto> Invoices { get; set; } = [];
    public required decimal TotalPrice { get; set; }
    public int OrderRowsCount => OrderRows.Count;
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class OrderNotificationDto
{
    public required long Id { get; set; }
    public required string OrderStatus { get; set; }
    public required string Destination { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class OrderTransportEventDto
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

public class OrderInvoiceDto
{
    public required long Id { get; set; }
    public required string Status { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class OrderCustomerDto
{
    public required long Id { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}