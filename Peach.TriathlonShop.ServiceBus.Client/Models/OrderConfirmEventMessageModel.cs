namespace Peach.TriathlonShop.ServiceBus.Client.Models;

public class OrderConfirmEventMessageModel
{
    public const string EventType = "triathlonshop.order.confirm";

    public required long OrderId { get; set; }
    public required DateTimeOffset EventCreated { get; set; }
}
