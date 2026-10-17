using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Peach.TriathlonShop.Database.Models;
using Peach.TriathlonShop.Database.Models.Enums;
using Peach.TriathlonShop.FunctionApp.Extensions;
using Peach.TriathlonShop.FunctionApp.Models;
using Peach.TriathlonShop.FunctionApp.Queues;
using Peach.TriathlonShop.FunctionApp.Queues.Models;
using Peach.TriathlonShop.FunctionApp.ServiceBus.Models;
using Peach.TriathlonShop.ServiceBus.Client.Models;

namespace Peach.TriathlonShop.FunctionApp.ServiceBus
{
    public class OrderConfirmEvent()
    {
        [Function(nameof(OrderConfirmEvent))]
        public async Task<OrderConfirmEventOutput> Run([ServiceBusTrigger("triathlonshop.order.confirm", "triathlonshop-functionapp", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message)
        {
            var @event = message.Body.ToObjectFromJson<OrderConfirmEventMessageModel>();

            if (@event == null)
                throw new InvalidOperationException("Failed to deserialize message.");

            return new OrderConfirmEventOutput
            {
                SignalRMessage = new SignalRMessageAction(SignalRHubName.OrdersHub)
                {
                    Arguments =
                    [
                        new
                        {
                            Message = "Order confirmed..",
                            Timestamp = DateTimeOffset.UtcNow,
                        }
                    ]
                },
                SendNotificationQueueMessage = new SendNotificationModel
                {
                    OrderId = @event.OrderId,
                    Status = OrderStatus.Confirmed
                },
                CreateTransportOrderRequestQueueMessage = @event.OrderId.ToString(),
                CreateInvoiceQueueMessage = new CreateInvoiceModel
                {
                    OrderId = @event.OrderId,
                    InvoiceStatus = InvoiceStatus.Created,
                }
            };
        }
    }
}

namespace Peach.TriathlonShop.FunctionApp.ServiceBus.Models
{
    public class OrderConfirmEventOutput
    {
        [SignalROutput(HubName = SignalRHubName.OrdersHub)]
        public SignalRMessageAction? SignalRMessage { get; set; }

        [QueueOutput(SendNotification.QueueName)]
        public required SendNotificationModel SendNotificationQueueMessage { get; set; }

        [QueueOutput(CreateTransportOrderRequest.QueueName)]
        public required string CreateTransportOrderRequestQueueMessage { get; set; }

        [QueueOutput(CreateInvoice.QueueName)]
        public required CreateInvoiceModel CreateInvoiceQueueMessage { get; set; }
    }

}