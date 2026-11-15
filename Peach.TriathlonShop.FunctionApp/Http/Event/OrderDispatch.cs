using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models.Enums;
using Peach.TriathlonShop.FunctionApp.Services;
using Peach.TriathlonShop.ServiceBus.Client.Models;

namespace Peach.TriathlonShop.FunctionApp.Http.Event;

public class OrderDispatch(ILogger<OrderDispatch> logger, TriathlonShopDbContext context, ServiceBusClient serviceBusClient, SignalRNotifier signalRNotifier)
{
    [Function(nameof(OrderDispatch))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders/{id}/dispatch")] HttpRequest req, long id)
    {
        var order = await context.Orders.FindAsync(id);

        if (order == null)
            return new NotFoundObjectResult($"Order with ID {id} not found.");

        if (order.Status != OrderStatus.Confirmed)
            return new BadRequestObjectResult($"Order with ID {id} cannot be dispatched because its status is not {OrderStatus.Confirmed.ToString()}.");

        await signalRNotifier.SendOrderMessageAsync(id, $"Event: Start publish event to topic {OrderDispatchEventMessageModel.EventType}.");

        try
        {
            var sender = serviceBusClient.CreateSender(OrderDispatchEventMessageModel.EventType);

            var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(new OrderDispatchEventMessageModel
            {
                OrderId = order.Id,
                EventCreated = DateTimeOffset.UtcNow,
            }));

            await sender.SendMessageAsync(message);
        }
        catch (Exception ex)
        {
            await signalRNotifier.SendOrderMessageAsync(id, $"Error: Failed to publish event to topic {OrderDispatchEventMessageModel.EventType}. Exception: {ex.Message}");
            throw new Exception($"Failed to publish event to topic {OrderDispatchEventMessageModel.EventType}.", ex);
        }

        await signalRNotifier.SendOrderMessageAsync(id, $"Event: Finish publish event to topic {OrderDispatchEventMessageModel.EventType}.");

        order.Status = OrderStatus.Dispatched;
        await context.SaveChangesAsync();

        await signalRNotifier.SendOrderMessageAsync(id, $"Order update: Status changed to {OrderStatus.Dispatched.ToString()}.", isUpdate: true);

        return new OkResult();
    }
}