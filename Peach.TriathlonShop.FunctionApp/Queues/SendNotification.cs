using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;
using Peach.TriathlonShop.Database.Models.Enums;
using Peach.TriathlonShop.FunctionApp.Queues.Models;
using Peach.TriathlonShop.FunctionApp.Services;

namespace Peach.TriathlonShop.FunctionApp.Queues
{
    public class SendNotification(ILogger<SendNotification> logger, TriathlonShopDbContext context, SignalRNotifier signalRNotifier)
    {
        public const string QueueName = "send-notification";

        [Function(nameof(SendNotification))]
        public async Task Run([QueueTrigger(QueueName, Connection = "")] QueueMessage message)
        {
            var notification = message.Body.ToObjectFromJson<SendNotificationModel>();

            if (notification == null)
                throw new InvalidOperationException("Notification message is null or invalid.");

            await signalRNotifier.SendOrderMessageAsync(notification.OrderId, $"Notification: Starting sending notification to customer.");

            var order = await context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == notification.OrderId);

            if (order == null)
                throw new InvalidOperationException($"Order with ID {notification.OrderId} not found.");

            if (order.Customer == null)
                throw new InvalidOperationException($"Customer for order ID {notification.OrderId} not found.");

            if (string.IsNullOrWhiteSpace(order.Customer.Email))
                throw new InvalidOperationException($"Customer email for order ID {notification.OrderId} is not set.");

            switch (notification.Status)
            {
                case OrderStatus.Confirmed:

                    // To implement: Send notification order confirm

                    context.Notifications.Add(new Notification
                    {
                        OrderId = notification.OrderId,
                        OrderStatus = notification.Status,
                        Destination = order.Customer.Email,
                    });

                    await context.SaveChangesAsync();

                    break;
                case OrderStatus.Shipped:

                    // To implement: Send notification order shipped

                    context.Notifications.Add(new Notification
                    {
                        OrderId = notification.OrderId,
                        OrderStatus = notification.Status,
                        Destination = order.Customer.Email,
                    });

                    await context.SaveChangesAsync();

                    break;
                default:
                    logger.LogWarning("Unhandled order status {Status} for Order ID {OrderId}.", notification.Status, notification.OrderId);
                    break;
            }

            await signalRNotifier.SendOrderMessageAsync(notification.OrderId, $"Notification: Finished sending notification to customer.", isUpdate: true);
        }
    }
}

namespace Peach.TriathlonShop.FunctionApp.Queues.Models
{
    public class SendNotificationModel
    {
        public required long OrderId { get; set; }
        public required OrderStatus Status { get; set; }
    }
}