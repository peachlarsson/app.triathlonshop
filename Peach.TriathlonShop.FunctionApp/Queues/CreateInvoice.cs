using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Logging;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;
using Peach.TriathlonShop.FunctionApp.Extensions;
using Peach.TriathlonShop.FunctionApp.Models;
using Peach.TriathlonShop.FunctionApp.Services;
using System.Text.Json;

namespace Peach.TriathlonShop.FunctionApp.Queues
{
    public class CreateInvoice(ILogger<CreateInvoice> logger, TriathlonShopDbContext context, SignalRNotifier signalRNotifier)
    {
        public const string QueueName = "create-invoice";

        [Function(nameof(CreateInvoice))]
        public async Task Run([QueueTrigger(QueueName, Connection = "")] QueueMessage message)
        {
            var model = JsonSerializer.Deserialize<CreateInvoiceModel>(message.Body.ToString());

            if (model == null)
                throw new InvalidOperationException("Failed to deserialize message.");

            await signalRNotifier.SendOrderMessageAsync(model.OrderId, $"Invoice: Starting creating invoice.");

            var order = await context.Orders.FindAsync(model.OrderId);

            if (order == null)
                throw new InvalidOperationException($"Order with ID {model.OrderId} not found.");

            // To implement: Create an invoice if not already created for the order

            context.Invoices.Add(new Invoice
            {
                OrderId = model.OrderId,
                Status = model.InvoiceStatus,
            });

            await context.SaveChangesAsync();

            await signalRNotifier.SendOrderMessageAsync(model.OrderId, $"Invoice: Finished creating invoice.", isUpdate: true);
        }
    }
}

namespace Peach.TriathlonShop.FunctionApp.Models
{
    public class CreateInvoiceModel
    {
        public required long OrderId { get; set; }
        public required InvoiceStatus InvoiceStatus { get; set; }
    }
}