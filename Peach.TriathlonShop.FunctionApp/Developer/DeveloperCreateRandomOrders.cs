using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Api.Contract.Developer.Models;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;
using Peach.TriathlonShop.Database.Models.Enums;
using Peach.TriathlonShop.FunctionApp.Http.Developer.Extensions;
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
                    Number = $"ORD-{Guid.NewGuid():N}"[..16],
                    Status = model.UseRandomOrderStatus ? CreateRandomOrdersExtensions.GetRandomOrderStatus() : OrderStatus.Created,
                    TotalPrice = 0,
                    DestinationLocation = context.Locations.OrderBy(l => Guid.NewGuid()).FirstOrDefault(),
                    PlacedAt = model.UseRandomCreatedDate ? CreateRandomOrdersExtensions.GetRandomDateTimeOffset() : DateTimeOffset.UtcNow,
                    CreatedAt = model.UseRandomCreatedDate ? CreateRandomOrdersExtensions.GetRandomDateTimeOffset() : DateTimeOffset.UtcNow,
                };

                if (context.Orders.Any(o => o.Number == order.Number))
                {
                    i--;
                    continue;
                }

                int articleCount = Random.Shared.Next(model.ArticlesCountIntervalStart ?? 1, model.ArticlesCountIntervalEnd ?? 5);
                var selectedArticleIds = new HashSet<long>();
                int lineNumber = 1;

                for (int j = 0; j < articleCount; j++)
                {
                    var article = articles[Random.Shared.Next(articles.Count)];

                    if (!selectedArticleIds.Add(article.Id))
                    {
                        continue;
                    }

                    int quantity = Random.Shared.Next(model.QuantityIntervalStart ?? 1, model.QuantityIntervalEnd ?? 10);

                    order.OrderRows.Add(new OrderRow
                    {
                        OrderId = order.Id,
                        ArticleId = article.Id,
                        LineNumber = lineNumber++,
                        Quantity = quantity,
                        RowPrice = article.UnitPrice * quantity,
                    });
                }

                order.TotalPrice = order.OrderRows.Sum(or => or.RowPrice);
                context.Orders.Add(order);
                await context.SaveChangesAsync();

                await signalRNotifier.SendHomePageUpdateMessageAsync(SignalRHomePageUpdateType.OrderRows, true);

                if (model.UseRandomDelay)
                {
                    int delayMs = Random.Shared.Next(model.DelayMsIntervalStart ?? 0, model.DelayMsIntervalEnd ?? 3000);
                    await Task.Delay(delayMs);
                }
            }

            return new OkResult();
        }
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

        public static OrderStatus GetRandomOrderStatus()
        {
            var values = Enum.GetValues(typeof(OrderStatus));

            return (OrderStatus)values.GetValue(Random.Shared.Next(values.Length))!;
        }

        public static DateTimeOffset GetRandomDateTimeOffset()
        {
            var start = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

            var end = DateTimeOffset.UtcNow;

            var range = (end - start).Days;

            return start.AddDays(Random.Shared.Next(range));
        }
    }
}