using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;
using Peach.TriathlonShop.Database.Models.Enums;
using Peach.TriathlonShop.FunctionApp.Models;
using Peach.TriathlonShop.FunctionApp.Queues;
using Peach.TriathlonShop.FunctionApp.Queues.Models;
using Peach.TriathlonShop.FunctionApp.ServiceBus.Models;
using Peach.TriathlonShop.FunctionApp.Services;
using Peach.TriathlonShop.ServiceBus.Client.Models;

namespace Peach.TriathlonShop.FunctionApp.ServiceBus
{
    public class OrderShipEvent(ILogger<OrderShipEvent> logger, SignalRNotifier signalRNotifier)
    {
        [Function(nameof(OrderShipEvent))]
        public async Task<OrderShippedOutput> Run([ServiceBusTrigger("triathlonshop.order.ship", "triathlonshop-functionapp")] ServiceBusReceivedMessage message)
        {
            var @event = message.Body.ToObjectFromJson<OrderShipEventMessageModel>();

            if (@event is null)
                throw new InvalidOperationException("Failed to deserialize event message.");

            await signalRNotifier.SendOrderMessageAsync(@event.OrderId, "Order shipped: Received order shipped event from Service Bus.");

            return new OrderShippedOutput
            {
                SendNotificationQueueMessage = new SendNotificationModel
                {
                    OrderId = @event.OrderId,
                    Status = OrderStatus.Shipped
                },
                CreateInvoiceQueueMessage = new CreateInvoiceModel
                {
                    OrderId = @event.OrderId,
                    InvoiceStatus = InvoiceStatus.Sent,
                },
            };
        }
    }
}

namespace Peach.TriathlonShop.FunctionApp.ServiceBus.Models
{
    public class OrderShippedOutput
    {
        [QueueOutput(SendNotification.QueueName)]
        public required SendNotificationModel SendNotificationQueueMessage { get; set; }

        [QueueOutput(CreateInvoice.QueueName)]
        public required CreateInvoiceModel CreateInvoiceQueueMessage { get; set; }
    }
}
