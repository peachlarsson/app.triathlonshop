using Peach.TriathlonShop.Api.Contract.Enums.Common;
using Peach.TriathlonShop.Api.Contract.Models.Orders;

namespace Peach.TriathlonShop.Api.Contract.Models;

public class OrderResponseDto
{
    public required long Id { get; set; }
    public required OrderStatus Status { get; set; }
    public required string Number { get; set; }
    public required CustomerDto Customer { get; set; }
    public required LocationDto DestinationLocation { get; set; }
    public List<OrderRowDto> OrderRows { get; set; } = [];
    public List<NotificationDto> Notifications { get; set; } = [];
    //public List<OrderTransportEventDto> TransportEvents { get; set; } = [];
    public List<InvoiceDto> Invoices { get; set; } = [];
    public required decimal TotalPrice { get; set; }
    public int OrderRowsCount => OrderRows.Count;
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
