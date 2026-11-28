using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.FunctionApp.Business.ResponseDtos;

public class InvoiceDto
{
    public required long Id { get; set; }
    public required long OrderId { get; set; }
    public required InvoiceStatus Status { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}
