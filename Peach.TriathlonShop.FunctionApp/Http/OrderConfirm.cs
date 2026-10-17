using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.SignalR.Management;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models.Enums;
using Peach.TriathlonShop.FunctionApp.Extensions;
using Peach.TriathlonShop.ServiceBus.Client.Models;

namespace Peach.TriathlonShop.FunctionApp.Http;

public class OrderConfirm(TriathlonShopDbContext context, ServiceBusClient serviceBusClient, IServiceManager signalRServiceManager)
{
    [Function(nameof(OrderConfirm))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "order/{id}/confirm")] HttpRequest req, long id)
    {
        var order = await context.Orders.FindAsync(id);

        if (order == null)
            return new NotFoundObjectResult($"Order with ID {id} not found.");

        if (order.Status != OrderStatus.Created)
            return new BadRequestObjectResult($"Order with ID {id} cannot be confirmed because its status is not {OrderStatus.Created.ToString()}.");

        await using (var hub = await signalRServiceManager.CreateHubContextAsync(SignalRHubName.OrdersHub))
        {
            await hub.Clients.All.SendCoreAsync(
                $"Orders",
                [
                    new
                        {
                            Message = $"Publishing event {OrderConfirmEventMessageModel.EventType}....",
                            Timestamp = DateTimeOffset.UtcNow
                        }
                ]);
        }

        var sender = serviceBusClient.CreateSender(OrderConfirmEventMessageModel.EventType);

        var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(new OrderConfirmEventMessageModel
        {
            OrderId = order.Id,
            EventCreated = DateTimeOffset.UtcNow,
        }));

        await sender.SendMessageAsync(message);

        await using (var hub = await signalRServiceManager.CreateHubContextAsync(SignalRHubName.OrdersHub))
        {
            await hub.Clients.All.SendCoreAsync(
                $"Orders",
                [
                    new
                        {
                            Message = $"Published event {OrderConfirmEventMessageModel.EventType}.",
                            Timestamp = DateTimeOffset.UtcNow
                        }
                ]);
        }

        order.Status = OrderStatus.Confirmed;
        await context.SaveChangesAsync();

        return new OkResult();
    }
}