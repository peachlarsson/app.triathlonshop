using Peach.TriathlonShop.Api.Contract.Enums.Common;

namespace Peach.TriathlonShop.Api.Contract.Models.Orders;

public class InvoiceDto
{
    public required long Id { get; set; }
    public required long OrderId { get; set; }
    public required InvoiceStatus Status { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}
