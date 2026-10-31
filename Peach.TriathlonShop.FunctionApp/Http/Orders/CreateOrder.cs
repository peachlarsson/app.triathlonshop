using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Peach.TriathlonShop.Api.Contract.Models.Orders;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;
using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.FunctionApp.Http.Orders;

public class CreateOrder(TriathlonShopDbContext context)
{
    [Function(nameof(CreateOrder))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] HttpRequest req)
    {
        var request = await req.ReadFromJsonAsync<CreateOrderRequestDto>();

        if (request == null)
        {
            return new BadRequestObjectResult("Invalid request body.");
        }

        if (request.OrderRows == null || !request.OrderRows.Any())
        {
            return new BadRequestObjectResult("Order must contain at least one order row.");
        }

        string orderNumber = $"ORD-{Guid.NewGuid():N}"[..16];

        while (context.Orders.Any(o => o.Number == orderNumber))
        {
            orderNumber = $"ORD-{Guid.NewGuid():N}"[..16];
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            DestinationLocationId = 1, // TODO
            Number = orderNumber,
            Status = OrderStatus.Created,
            TotalPrice = 0,
            PlacedAt = DateTime.UtcNow,
        };

        context.Orders.Add(order);

        int lineNumber = 1;

        foreach (var newArticle in request.OrderRows)
        {
            var article = context.Articles.Find(newArticle.ArticleId);

            if (article == null)
            {
                return new BadRequestObjectResult($"Article with ID {newArticle.ArticleId} not found.");
            }

            order.OrderRows.Add(new OrderRow
            {
                LineNumber = lineNumber,
                ArticleId = newArticle.ArticleId,
                Quantity = newArticle.Quantity,
                RowPrice = article.UnitPrice * newArticle.Quantity,
            });

            lineNumber++;
        }

        order.TotalPrice = order.OrderRows.Sum(or => or.RowPrice);

        await context.SaveChangesAsync();

        return new OkResult();
    }
}
