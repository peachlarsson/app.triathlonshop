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
using Peach.TriathlonShop.ServiceBus.Client.Models;

namespace Peach.TriathlonShop.FunctionApp.ServiceBus
{
    public class OrderShippedEvent(ILogger<OrderShippedEvent> logger, TriathlonShopDbContext context)
    {
        [Function(nameof(OrderShippedEvent))]
        public async Task<OrderShippedOutput> Run([ServiceBusTrigger("triathlonshop.order.shipped", "triathlonshop", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message)
        {
            var @event = message.Body.ToObjectFromJson<OrderShippedEventMessageModel>();

            if (@event is null)
                throw new InvalidOperationException("Failed to deserialize event message.");

            var order = await context.Orders.FirstAsync(o => o.Id == @event.OrderId);

            return new OrderShippedOutput
            {
                SendNotificationQueueMessage = new SendNotificationModel
                {
                    OrderId = order.Id,
                    Status = OrderStatus.Shipped
                },
                CreateInvoiceQueueMessage = new CreateInvoiceModel
                {
                    OrderId = order.Id,
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
