using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Api.Contract.Models;
using Peach.TriathlonShop.Api.Contract.Models.Stock;
using Peach.TriathlonShop.Database;

namespace Peach.TriathlonShop.FunctionApp.Http.Stock;

public class GetAllWarehouseStock(TriathlonShopDbContext context)
{
    [Function(nameof(GetAllWarehouseStock))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "stock/all-warehouses")] HttpRequest req)
    {
        var warehouses = await context.Stocks
            .AsNoTracking()
            .GroupBy(s => new
            {
                s.WarehouseId,
                s.Warehouse.WarehouseName,
                s.Warehouse.Location.CountryCode,
                s.Warehouse.Location.CountryName,
                s.Warehouse.Location.City
            })
            .Select(g => new WarehouseStockDto
            {
                WarehouseId = g.Key.WarehouseId,
                WarehouseName = g.Key.WarehouseName,
                Location = new LocationDto
                {
                    CountryCode = g.Key.CountryCode,
                    CountryName = g.Key.CountryName,
                    City = g.Key.City
                },
                Articles = g.Select(a => new ArticleStockDto
                {
                    ArticleId = a.ArticleId,
                    ArticleName = a.Article.Name,
                    ArticleNumber = a.Article.Number,
                    Quantity = a.Quantity
                }).ToList()
            })
            .ToListAsync();

        var stock = new GetAllWarehouseStockDto
        {
            Warehouses = warehouses,
            TotalWarehouses = warehouses.Count
        };

        return new OkObjectResult(stock);
    }
}
