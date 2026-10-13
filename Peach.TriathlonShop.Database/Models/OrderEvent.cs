using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.Database.Models;

public class OrderEvent : BaseEntity
{
    public required long OrderId { get; set; }
    public required OrderStatus Status { get; set; }
    public required string Description { get; set; }

    public Order? Order { get; set; } = null!;
}
