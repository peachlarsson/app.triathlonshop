using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
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

                return new DashboardWarehouseDto
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


        var result = new GetStockDashboardResponse
        {
            Warehouses = warehouses,
        };

        return new OkObjectResult(result);
    }
}

public class GetStockDashboardResponse
{
    public List<DashboardWarehouseDto> Warehouses { get; set; } = [];

}

public class DashboardWarehouseDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalArticles { get; set; }
    public int InStock { get; set; }
    public int OutOfStock { get; set; }
    public int AwaitingDispatch { get; set; }
    public int Shipped { get; set; }
}