namespace Peach.TriathlonShop.ServiceBus.Client.Models;

public class OrderDispatchEventMessageModel
{
    public const string EventType = "triathlonshop.order.dispatch";

    public required long OrderId { get; set; }
    public required DateTimeOffset EventCreated { get; set; }
}
