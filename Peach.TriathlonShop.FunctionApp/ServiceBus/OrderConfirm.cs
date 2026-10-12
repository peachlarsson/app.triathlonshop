using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Peach.TriathlonShop.FunctionApp.ServiceBus;

public class OrderConfirm()
{
    [Function(nameof(OrderConfirm))]
    public async Task Run([ServiceBusTrigger("triathlonshop.order.confirm", "triathlonshop", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message)
    {
    }
}