namespace Peach.TriathlonShop.Database.Models;

public class OrderItem : BaseEntity
{
    public long CustomerId { get; set; }
    public long ProductId { get; set; }

    public required Customer Customer { get; set; }
    public required Product Product { get; set; }
}
