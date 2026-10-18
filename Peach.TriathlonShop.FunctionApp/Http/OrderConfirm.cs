using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models.Enums;
using Peach.TriathlonShop.FunctionApp.Services;
using Peach.TriathlonShop.ServiceBus.Client.Models;

namespace Peach.TriathlonShop.FunctionApp.Http;

public class OrderConfirm(ILogger<OrderConfirm> logger, TriathlonShopDbContext context, ServiceBusClient serviceBusClient, SignalRNotifier signalRNotifier)
{
    [Function(nameof(OrderConfirm))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "order/{id}/confirm")] HttpRequest req, long id)
    {
        var order = await context.Orders.FindAsync(id);

        if (order == null)
            return new NotFoundObjectResult($"Order with ID {id} not found.");

        if (order.Status != OrderStatus.Created)
            return new BadRequestObjectResult($"Order with ID {id} cannot be confirmed because its status is not {OrderStatus.Created.ToString()}.");

        await signalRNotifier.SendOrderMessageAsync(id, $"Event: Start publish event to topic {OrderConfirmEventMessageModel.EventType}.");

        try
        {
            var sender = serviceBusClient.CreateSender(OrderConfirmEventMessageModel.EventType);

            var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(new OrderConfirmEventMessageModel
            {
                OrderId = order.Id,
                EventCreated = DateTimeOffset.UtcNow,
            }));

            await sender.SendMessageAsync(message);
        }
        catch (Exception ex)
        {
            await signalRNotifier.SendOrderMessageAsync(id, $"Error: Failed to publish event to topic {OrderConfirmEventMessageModel.EventType}. Exception: {ex.Message}");
            throw new Exception($"Failed to publish event to topic {OrderConfirmEventMessageModel.EventType}.", ex);
        }

        await signalRNotifier.SendOrderMessageAsync(id, $"Event: Finish publish event to topic {OrderConfirmEventMessageModel.EventType}.");

        order.Status = OrderStatus.Confirmed;
        await context.SaveChangesAsync();

        await signalRNotifier.SendOrderMessageAsync(id, $"Order update: Status changed to {OrderStatus.Confirmed.ToString()}.", isUpdate: true);

        return new OkResult();
    }
}