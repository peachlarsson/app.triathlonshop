using Peach.TriathlonShop.Api.Contract.Enums.Common;

namespace Peach.TriathlonShop.Api.Contract.Models.Orders;

public class NotificationDto
{
    public required long OrderId { get; set; }
    public required OrderStatus OrderStatus { get; set; }
    public required string Destination { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}
