using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;
using Peach.TriathlonShop.Database.Models.Enums;
using Peach.TriathlonShop.FunctionApp.Http.Developer.Extensions;
using Peach.TriathlonShop.FunctionApp.Http.Developer.Models;
using Peach.TriathlonShop.FunctionApp.Services;
using Peach.TriathlonShop.FunctionApp.Services.Models;
using System.Text.Json;

namespace Peach.TriathlonShop.FunctionApp.Developer
{
    public class DeveloperCreateRandomOrders(TriathlonShopDbContext context, SignalRNotifier signalRNotifier)
    {
        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        [Function(nameof(DeveloperCreateRandomOrders))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "developer/orders/random")] HttpRequest req)
        {
            using var reader = new StreamReader(req.Body);
            var requestBody = await reader.ReadToEndAsync();

            var model = JsonSerializer.Deserialize<CreateRandomOrdersModel>(requestBody, jsonSerializerOptions);

            if (model == null)
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            if (!model.IsValidRequestBody())
                return new BadRequestResult();

            var articles = await context.Articles.ToListAsync();
            var customers = await context.Customers.ToListAsync();

            for (int i = 0; i < model.OrderRows; i++)
            {
                var order = new Order
                {
                    CustomerId = customers[Random.Shared.Next(customers.Count)].Id,
                    Number = $"ORD-{Guid.NewGuid()}",
                    Status = OrderStatus.Created,
                    TotalPrice = 0,
                };

                if (context.Orders.Any(o => o.Number == order.Number))
                {
                    i--;
                    continue;
                }

                context.Orders.Add(order);
                await context.SaveChangesAsync();

                await signalRNotifier.SendHomePageUpdateMessageAsync(SignalRHomePageUpdateType.Orders, true);

                int articleCount = Random.Shared.Next(model.ArticlesCountIntervalStart ?? 1, model.ArticlesCountIntervalEnd ?? 5);

                for (int j = 0; j < articleCount; j++)
                {
                    int articleId = Random.Shared.Next(articles.Count);
                    int quantity = Random.Shared.Next(model.QuantityIntervalStart ?? 1, model.QuantityIntervalEnd ?? 10);

                    var orderRow = new OrderRow
                    {
                        ArticleId = articles[articleId].Id,
                        LineNumber = j + 1,
                        Quantity = quantity,
                        RowPrice = articles[articleId].UnitPrice * quantity,
                        OrderId = order.Id,
                    };

                    if (context.OrderRows.Any(or => or.ArticleId == orderRow.ArticleId && or.Quantity == orderRow.Quantity && or.RowPrice == orderRow.RowPrice))
                    {
                        j--;
                        continue;
                    }

                    context.OrderRows.Add(orderRow);
                    await context.SaveChangesAsync();

                    await signalRNotifier.SendHomePageUpdateMessageAsync(SignalRHomePageUpdateType.OrderRows, true);
                }

                order.TotalPrice = context.OrderRows.Where(or => or.OrderId == order.Id).Sum(or => or.RowPrice);
                await context.SaveChangesAsync();
            }

            return new OkResult();
        }
    }
}

namespace Peach.TriathlonShop.FunctionApp.Http.Developer.Models
{
    public class CreateRandomOrdersModel
    {
        public int? OrderRows { get; set; }
        public int? ArticlesCountIntervalStart { get; set; }
        public int? ArticlesCountIntervalEnd { get; set; }
        public int? QuantityIntervalStart { get; set; }
        public int? QuantityIntervalEnd { get; set; }
    }
}

namespace Peach.TriathlonShop.FunctionApp.Http.Developer.Extensions
{
    public static class CreateRandomOrdersExtensions
    {
        public static bool IsValidRequestBody(this CreateRandomOrdersModel model)
        {
            return model != null && model.OrderRows != null && model.OrderRows > 0;
        }
    }
}