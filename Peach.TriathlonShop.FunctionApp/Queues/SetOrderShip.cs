//using Azure.Storage.Queues.Models;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.Logging;
//using Peach.TriathlonShop.Database;
//using Peach.TriathlonShop.Database.Models.Enums;
//using Peach.TriathlonShop.FunctionApp.Services;

//namespace Peach.TriathlonShop.FunctionApp.Queues
//{
//    public class SetOrderShip(ILogger<SetOrderShip> logger, TriathlonShopDbContext context, SignalRNotifier signalRNotifier)
//    {
//        public const string QueueName = "set-order-ship";

//        [Function(nameof(SetOrderShip))]
//        public async Task Run([QueueTrigger(QueueName)] QueueMessage message)
//        {
//            long orderId = long.Parse(message.MessageText);

//            await signalRNotifier.SendOrderMessageAsync(orderId, $"Order shipped: Starting set order shipped.");

//            var order = await context.Orders.FindAsync(orderId);

//            if (order == null)
//                throw new InvalidOperationException($"Order with ID {orderId} not found.");
            
//            order.Status = OrderStatus.Shipped;

//            await context.SaveChangesAsync();

//            await signalRNotifier.SendOrderMessageAsync(orderId, $"Order shipped: Finished set order shipped.", isUpdate: true);
//        }
//    }
//}
