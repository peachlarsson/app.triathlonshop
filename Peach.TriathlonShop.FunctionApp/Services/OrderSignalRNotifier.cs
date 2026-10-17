using Microsoft.Azure.SignalR.Management;
using Peach.TriathlonShop.FunctionApp.Extensions;

namespace Peach.TriathlonShop.FunctionApp.Services;

public class SignalRNotifier(IServiceManager signalRServiceManager)
{
    public async Task SendOrderMessageAsync(long orderId, string message, bool isUpdate = false)
    {
        await using var hub = await signalRServiceManager.CreateHubContextAsync(SignalRHubName.OrdersHub);

        await hub.Clients.All.SendCoreAsync(
            $"order{orderId}",
            [
                new
                {
                    Message = message,
                    Update = isUpdate,
                    Timestamp = DateTimeOffset.UtcNow
                }
            ]);
    }
}