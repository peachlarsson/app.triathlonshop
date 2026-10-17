using Microsoft.Azure.SignalR.Management;
using Peach.TriathlonShop.FunctionApp.Extensions;
using Peach.TriathlonShop.FunctionApp.Services.Models;

namespace Peach.TriathlonShop.FunctionApp.Services
{
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

        public async Task SendHomePageUpdateMessageAsync(SignalRHomePageUpdateType updateType, bool isUpdate = false)
        {
            await using var hub = await signalRServiceManager.CreateHubContextAsync(SignalRHubName.HomePageHub);
            await hub.Clients.All.SendCoreAsync(
                "homepage",
                [
                    new
                    {
                        Message = updateType.ToString(),
                        Update = isUpdate,
                        Timestamp = DateTimeOffset.UtcNow
                    }
                ]);
        }
    }
}

namespace Peach.TriathlonShop.FunctionApp.Services.Models
{
    public enum SignalRHomePageUpdateType
    {
        None,
        Orders,
        OrderRows,
        Articles,
        Customers,
    }
}