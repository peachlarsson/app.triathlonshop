namespace Peach.TriathlonShop.Database.Models;

public class Order : BaseEntity
{
    public required long CustomerId { get; set; }
    public required string OrderNumber { get; set; }
    public required decimal TotalPrice { get; set; }
    public List<OrderRow> OrderRows { get; set; } = [];

    public required Customer Customer { get; set; }
}
