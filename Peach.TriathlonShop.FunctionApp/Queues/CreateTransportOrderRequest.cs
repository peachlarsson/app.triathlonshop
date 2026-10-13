using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.FunctionApp.Queues;

public class CreateTransportOrderRequest(ILogger<CreateTransportOrderRequest> logger, TriathlonShopDbContext context)
{
    public const string QueueName = "create-transport-order-request";

    [Function(nameof(CreateTransportOrderRequest))]
    public async Task Run([QueueTrigger(QueueName, Connection = "")] QueueMessage message)
    {
        var orderId = long.Parse(message.MessageText);

        var order = await context.Orders.FindAsync(orderId);

        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found.");

        // To implement: Send transport request with consigment number

        context.TransportOrders.Add(new TransportOrder
        {
            OrderId = orderId,
            Destination = "Some Destination",
            Status = Database.Models.Enums.TransportStatus.Requested,
            ConsignmentNumber = $"TR-{Guid.NewGuid()}",
            Carrier = "Some Carrier",
        });

        await context.SaveChangesAsync();
    }
}