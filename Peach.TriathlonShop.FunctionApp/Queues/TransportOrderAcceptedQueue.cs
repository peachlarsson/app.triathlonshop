using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models.Enums;
using Peach.TriathlonShop.FunctionApp.Http.Models;
using Peach.TriathlonShop.FunctionApp.Services;

namespace Peach.TriathlonShop.FunctionApp.Queues
{
    public class TransportOrderAcceptedQueue(ILogger<TransportOrderAcceptedQueue> logger, TriathlonShopDbContext context, SignalRNotifier signalRNotifier)
    {
        public const string QueueName = "transport-order-accepted";

        [Function(nameof(TransportOrderAcceptedQueue))]
        public async Task Run([QueueTrigger(QueueName, Connection = "")] QueueMessage message)
        {
            var transportInfo = message.Body.ToObjectFromJson<TransportOrderAcceptedEvent>();

            if (transportInfo == null)
                throw new InvalidOperationException("Transport order accepted event is null.");

            var transportOrder = await context.TransportOrders
                .Include(o => o.Order)
                .FirstOrDefaultAsync(o => o.ConsignmentNumber == transportInfo.ConsignmentNumber);

            if (transportOrder == null)
                throw new InvalidOperationException($"Transport order with consignment number {transportInfo.ConsignmentNumber} not found.");

            await signalRNotifier.SendOrderMessageAsync(transportOrder.OrderId, $"Transport accepted: Received transport order.");

            transportOrder.Status = TransportStatus.Accepted;
            transportOrder.EstimatedDeliveryDate = transportInfo.EstimatedDeliveryDate;
            transportOrder.ConsignmentNumber = transportInfo.ConsignmentNumber;
            transportOrder.TrackingUrl = transportInfo.TrackingUrl;

            await context.SaveChangesAsync();

            await signalRNotifier.SendOrderMessageAsync(transportOrder.OrderId, $"Transport accepted: Finished updating transport order.");
        }
    }
}