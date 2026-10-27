using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Api.Contract.Models.Stock;
using Peach.TriathlonShop.Database;

namespace Peach.TriathlonShop.FunctionApp.Http.Stock;

public class GetAllWarehouseStock(TriathlonShopDbContext context)
{
    [Function(nameof(GetAllWarehouseStock))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "stock/all-warehouses")] HttpRequest req)
    {
        var stock = await context.Stocks
            .AsNoTracking()
            .GroupBy(s => s.WarehouseId)
            .Select(g => new WarehouseStockDto
            {
                WarehouseId = g.Key,
                Articles = g.Select(s => new ArticleStockDto
                {
                    ArticleId = s.ArticleId,
                    Quantity = s.Quantity
                }).ToList()
            })
            .ToListAsync();

        return new OkObjectResult(stock);
    }
}
