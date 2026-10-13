namespace Peach.TriathlonShop.ServiceBus.Client.Models;

public class OrderShippedEventMessageModel
{
    public const string EventType = "triathlonshop.order.shipped";

    public required long OrderId { get; set; }
    public required DateTimeOffset EventCreated { get; set; }
}