using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.Database.Models;

public class Notification : BaseEntity
{
    public required long OrderId { get; set; }
    public required OrderStatus OrderStatus { get; set; }
    public required string Destination { get; set; }

    public Order? Order { get; set; } = null!;
}
