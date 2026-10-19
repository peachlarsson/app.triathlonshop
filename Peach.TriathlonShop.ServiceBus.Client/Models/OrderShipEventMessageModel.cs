namespace Peach.TriathlonShop.ServiceBus.Client.Models;

public class OrderShipEventMessageModel
{
    public const string EventType = "triathlonshop.order.ship";

    public required long OrderId { get; set; }
    public required DateTimeOffset EventCreated { get; set; }
}