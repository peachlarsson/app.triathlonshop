namespace Peach.TriathlonShop.Database.Models;

public class OrderRow : BaseEntity
{
    public required long OrderId { get; set; }
    public required int LineNumber { get; set; }
    public required long ProductId { get; set; }
    public required int Quantity { get; set; }

    public required Order Order { get; set; }
    public required Product Product { get; set; }
}
