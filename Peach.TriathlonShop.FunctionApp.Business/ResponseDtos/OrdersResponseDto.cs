using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.FunctionApp.Business.ResponseDtos;

public class OrdersResponseDto
{
    public required long Id { get; set; }
    public required string Status { get; set; }
    public required string OrderNumber { get; set; }
    public CustomerDto? Customer { get; set; }
    public required LocationDto DestinationLocation { get; set; }
    public List<OrderRowDto> OrderRows { get; set; } = [];
    public List<NotificationDto> Notifications { get; set; } = [];
    public TransportOrderDto? TransportOrder { get; set; }
    public List<InvoiceDto> Invoices { get; set; } = [];
    public required decimal TotalPrice { get; set; }
    public int OrderRowsCount => OrderRows.Count;
    public required DateTimeOffset PlacedAt { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
