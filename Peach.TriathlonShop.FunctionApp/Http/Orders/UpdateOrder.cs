using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Api.Contract.Models;
using Peach.TriathlonShop.Api.Contract.Models.Orders;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.FunctionApp.Http.Orders;

public class UpdateOrder(TriathlonShopDbContext context)
{
    [Function(nameof(UpdateOrder))]
    public async Task<Response<HttpResponse>> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "orders/{id}")] HttpRequest req, long id)
    {
        var order = context.Orders.Find(id);

        if (order == null)
        {
            return new Response<HttpResponse>
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status404NotFound,
                ErrorMessage = "Order not found.",
            };
        }

        var request = await req.ReadFromJsonAsync<UpdateOrderRequestDto>();

        if (request == null)
        {
            return new Response<HttpResponse>
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status400BadRequest,
                ErrorMessage = "Invalid request body."
            };
        }

        foreach (var newArticle in request.AddOrderRows)
        {
            var article = context.Articles.Find(newArticle.ArticleId);

            int lineNumber = 1;

            if (article == null)
            {
                return new Response<HttpResponse>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = $"Article with ID {newArticle.ArticleId} not found."
                };
            }

            var test = await context.Orders
                .Include(o => o.OrderRows)
                .Where(o => o.Id == order.Id)
                .ToListAsync();

            /// Check if the order contains the article.
            if (order.OrderRows.Any(or => or.ArticleId == newArticle.ArticleId))
            {
                order.OrderRows.First(or => or.ArticleId == newArticle.ArticleId).Quantity += newArticle.Quantity;
                order.OrderRows.First(or => or.ArticleId == newArticle.ArticleId).RowPrice += article.UnitPrice * newArticle.Quantity;
            }
            else
            {
                order.OrderRows.Add(new OrderRow
                {
                    OrderId = order.Id,
                    LineNumber = lineNumber,
                    ArticleId = newArticle.ArticleId,
                    Quantity = newArticle.Quantity,
                    RowPrice = article.UnitPrice * newArticle.Quantity,
                });
            }

            lineNumber++;
        }

        //var test_check = await context.Orders
        //    .Include(o => o.OrderRows)
        //    .Where(o => o.Id == order.Id)
        //    .Select(o => o.TotalPrice)
        //    .FirstOrDefaultAsync();

        order.TotalPrice = order.OrderRows.Sum(or => or.RowPrice);

        await context.SaveChangesAsync();

        //var test_check_2 = await context.Orders
        //    .Include(o => o.OrderRows)
        //    .Where(o => o.Id == order.Id)
        //    .Select(o => o.TotalPrice)
        //    .FirstOrDefaultAsync();

        return new Response<HttpResponse>
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status200OK,
            Content = req.HttpContext.Response
        };
    }
}