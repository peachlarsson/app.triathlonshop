using Peach.TriathlonShop.Api.Contract.Enums.Common;

namespace Peach.TriathlonShop.Api.Contract.Models;

public class OrderResponseDto
{
    public required long Id { get; set; }
    public required OrderStatus Status { get; set; }
    public required string Number { get; set; }
    public required CustomerDto Customer { get; set; }
    public List<OrderRowDto> OrderRows { get; set; } = [];
    //public List<OrderNotificationDto> Notifications { get; set; } = [];
    //public List<OrderTransportEventDto> TransportEvents { get; set; } = [];
    //public List<OrderInvoiceDto> Invoices { get; set; } = [];
    public required decimal TotalPrice { get; set; }
    public int OrderRowsCount => OrderRows.Count;
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
