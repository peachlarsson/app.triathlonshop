using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;
using Peach.TriathlonShop.FunctionApp.Services;

namespace Peach.TriathlonShop.FunctionApp.Queues;

public class CreateTransportOrderRequest(ILogger<CreateTransportOrderRequest> logger, TriathlonShopDbContext context, SignalRNotifier signalRNotifier)
{
    public const string QueueName = "create-transport-order-request";

    [Function(nameof(CreateTransportOrderRequest))]
    public async Task Run([QueueTrigger(QueueName)] QueueMessage message)
    {
        var orderId = long.Parse(message.MessageText);

        await signalRNotifier.SendOrderMessageAsync(orderId, $"Transport request: Starting requesting transport.");

        var order = await context.Orders.FindAsync(orderId);

        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found.");

        // To implement: Send transport request with consigment number

        var transportOrder = new TransportOrder
        {
            OrderId = orderId,
            Destination = "Some Destination",
            Status = Database.Models.Enums.TransportStatus.Requested,
            ConsignmentNumber = $"TR-{Guid.NewGuid()}",
            Carrier = "Some Carrier",
        };

        context.TransportOrders.Add(transportOrder);

        await context.SaveChangesAsync();

        await signalRNotifier.SendOrderMessageAsync(orderId, $"Transport request: Finished requesting transport.", isUpdate: true);
    }
}