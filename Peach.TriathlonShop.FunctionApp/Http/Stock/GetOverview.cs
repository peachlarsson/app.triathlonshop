using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Api.Contract.Models.Stock;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.FunctionApp.Http.Stock;

public class GetOverview(TriathlonShopDbContext context)
{
    [Function(nameof(GetOverview))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "stock/overview")] HttpRequest req)
    {
        var stocks = await context.Stocks
            .Include(s => s.Warehouse)
                .ThenInclude(w => w.WarehouseShipToCountries)
            .ToListAsync();

        var orders = await context.Orders
            .Where(o => o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Shipped)
            .ToListAsync();

        var warehouses = stocks
            .GroupBy(s => new
            {
                s.WarehouseId,
                s.Warehouse.WarehouseName
            })
            .Select(g =>
            {
                var shipToCountryIds = g
                    .SelectMany(s => s.Warehouse.WarehouseShipToCountries)
                    .Select(c => c.CountryId)
                    .Distinct()
                    .ToHashSet();

                return new WarehouseOverviewDto
                {
                    Id = g.Key.WarehouseId,
                    Name = g.Key.WarehouseName,
                    TotalArticles = g.Count(),
                    InStock = g.Count(s => s.Quantity > 0),
                    OutOfStock = g.Count(s => s.Quantity == 0),
                    AwaitingDispatch = orders.Count(o =>
                        o.Status == OrderStatus.Confirmed &&
                        shipToCountryIds.Contains(o.DestinationCountryId)),
                    Shipped = orders.Count(o =>
                        o.Status == OrderStatus.Shipped &&
                        shipToCountryIds.Contains(o.DestinationCountryId)),
                };
            })
            .ToList();
        
        var monthlyStockSales = await context.Orders
            .Include(o => o.OrderRows)
            .Where(o => o.Status == OrderStatus.Shipped)
            .GroupBy(o => new { o.PlacedAt.Year, o.PlacedAt.Month })
            .Select(g => new MonthlyStockSales
            {
                Month = g.Key.Month,
                Sales = g.Sum(o => o.OrderRows.Sum(or => or.Quantity)),
                Purchased = 0, // TODO: Future implementation.
            })
            .ToListAsync();

        var result = new StockOverviewResponseDto
        {
            Warehouses = warehouses,
            MonthlyStockSales = monthlyStockSales,
        };

        return new OkObjectResult(result);
    }
}
