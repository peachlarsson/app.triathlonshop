using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Peach.TriathlonShop.FunctionApp.Queues;
using Peach.TriathlonShop.FunctionApp.ServiceBus.Models;
using Peach.TriathlonShop.FunctionApp.Services;
using Peach.TriathlonShop.ServiceBus.Client.Models;

namespace Peach.TriathlonShop.FunctionApp.ServiceBus
{
    public class OrderDispatchEvent(SignalRNotifier signalRNotifier)
    {
        [Function(nameof(OrderDispatchEvent))]
        public async Task<OrderDispatchEventOutput> Run([ServiceBusTrigger("triathlonshop.order.dispatch", "triathlonshop-functionapp")] ServiceBusReceivedMessage message)
        {
            var @event = message.Body.ToObjectFromJson<OrderDispatchEventMessageModel>();

            if (@event == null)
                throw new InvalidOperationException("Failed to deserialize message.");

            await signalRNotifier.SendOrderMessageAsync(@event.OrderId, $"Event: Received event from topic {OrderDispatchEventMessageModel.EventType}. Distributing work.");

            return new OrderDispatchEventOutput
            {
                CreateTransportOrderRequestQueueMessage = @event.OrderId.ToString(),
            };
        }
    }
}

namespace Peach.TriathlonShop.FunctionApp.ServiceBus.Models
{
    public class OrderDispatchEventOutput
    {
        [QueueOutput(CreateTransportOrderRequest.QueueName)]
        public required string CreateTransportOrderRequestQueueMessage { get; set; }
    }

}