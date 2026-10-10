using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.App.Server.Controllers.Models;

public class OrderRowDto
{
    public required long Id { get; set; }
    public required long OrderId { get; set; }
    public required string OrderNumber { get; set; }
    public required int LineNumber { get; set; }
    public required long ArticleId { get; set; }
    public required string ArticleName { get; set; }
    public required decimal UnitPrice { get; set; }
    public required int Quantity { get; set; }
    public required decimal RowPrice { get; set; }
    public required long CustomerId { get; set; }
    public required string CustomerName { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
