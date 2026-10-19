using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models.Enums;
using Peach.TriathlonShop.FunctionApp.Http.Models;
using Peach.TriathlonShop.FunctionApp.Queues;
using Peach.TriathlonShop.FunctionApp.Services;
using Peach.TriathlonShop.ServiceBus.Client.Models;

namespace Peach.TriathlonShop.FunctionApp.Http
{
    public class OrderShip(TriathlonShopDbContext context, SignalRNotifier signalRNotifier, ServiceBusClient serviceBusClient)
    {
        [Function(nameof(OrderShip))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "order/ship/{orderId}")] HttpRequest req, long orderId)
        {
            await signalRNotifier.SendOrderMessageAsync(orderId, "Order shipped: Started set status to Shipped");

            try
            {
                var sender = serviceBusClient.CreateSender(OrderShipEventMessageModel.EventType);

                var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(new OrderShipEventMessageModel
                {
                    OrderId = orderId,
                    EventCreated = DateTimeOffset.UtcNow,
                }));

                await sender.SendMessageAsync(message);

                await signalRNotifier.SendOrderMessageAsync(orderId, $"Order shipped: Published event to topic {OrderShipEventMessageModel.EventType}");
            }
            catch (Exception ex)
            {
                await signalRNotifier.SendOrderMessageAsync(orderId, $"Error: Failed to publish event to topic {OrderShipEventMessageModel.EventType}. Exception: {ex.Message}");
                throw new Exception($"Failed to publish event to topic {OrderShipEventMessageModel.EventType}.", ex);
            }

            var order = await context.Orders.FirstAsync(o => o.Id == orderId);

            order.Status = OrderStatus.Shipped;
            await context.SaveChangesAsync();

            await signalRNotifier.SendOrderMessageAsync(orderId, "Order shipped: Finished set status to Shipped", isUpdate: true);

            return new OkResult();
        }
    }
}
