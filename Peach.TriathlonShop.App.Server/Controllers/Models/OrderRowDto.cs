using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.App.Server.Controllers.Models;

public class OrderRowDto
{
    public required long Id { get; set; }
    public required long OrderId { get; set; }
    public required string OrderNumber { get; set; }
    public required int LineNumber { get; set; }
    public required long ProductId { get; set; }
    public required string ProductName { get; set; }
    public required decimal UnitPrice { get; set; }
    public required int Quantity { get; set; }
}
