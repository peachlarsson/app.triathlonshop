namespace Peach.TriathlonShop.Database.Models;

public class OrderRow : BaseEntity
{
    public long OrderId { get; set; }
    public required int LineNumber { get; set; }
    public required long ArticleId { get; set; }
    public required int Quantity { get; set; }
    public required decimal RowPrice { get; set; }

    public Order? Order { get; set; } = null!;
    public Article? Article { get; set; } = null!;
}
