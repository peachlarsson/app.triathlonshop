using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.App.Server.Developer.Extensions;
using Peach.TriathlonShop.App.Server.Developer.Models;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;
using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.App.Server.Developer
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeveloperController(TriathlonShopDbContext context) : ControllerBase
    {
        [HttpPost("orders/random/")]
        public async Task<IActionResult> CreateOrder(CreateRandomOrdersModel model)
        {
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
                }

                order.TotalPrice = context.OrderRows.Where(or => or.OrderId == order.Id).Sum(or => or.RowPrice);
                await context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost("articles/random/{items}")]
        public async Task<IActionResult> CreateArticle(int items)
        {
            List<Article> articles = [];

            for (int i = 0; i < items; i++)
            {
                var article = new Article
                {
                    Number = $"ART-{Guid.NewGuid()}",
                    Name = ExtensionsCollection.GetRandomArticle(),
                    UnitPrice = Random.Shared.Next(1, 100)
                };

                if (context.Articles.Any(a => a.Name == article.Name))
                {
                    i--;
                    continue;
                }

                articles.Add(article);
            }

            context.Articles.AddRange(articles);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}

namespace Peach.TriathlonShop.App.Server.Developer.Models
{
    public class ArticleModel
    {
        public required string Name { get; set; }
        public required decimal UnitPrice { get; set; }
    }

    public class CreateRandomOrdersModel
    {
        public int? OrderRows { get; set; }
        public int? ArticlesCountIntervalStart { get; set; }
        public int? ArticlesCountIntervalEnd { get; set; }
        public int? QuantityIntervalStart { get; set; }
        public int? QuantityIntervalEnd { get; set; }
    }
}