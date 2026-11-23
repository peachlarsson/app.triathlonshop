using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Api.Contract.Models;
using Peach.TriathlonShop.Api.Contract.Models.Stock;
using Peach.TriathlonShop.Database;

namespace Peach.TriathlonShop.FunctionApp.Http.Stock;

public class WarehousesHttpGet(TriathlonShopDbContext context)
{
    [Function(nameof(WarehousesHttpGet))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "stock/warehouses")] HttpRequest req)
    {
        var warehouses = await context.Warehouses
            .Select(w => new WarehouseStockDto
            {
                WarehouseId = w.Id,
                WarehouseName = w.WarehouseName,
                Location = new LocationDto
                {
                    CountryCode = w.Location.Country.Code,
                    CountryName = w.Location.Country.Name,
                    CityName = w.Location.City.Name,
                    Latitude = w.Location.City.Latitude,
                    Longitude = w.Location.City.Longitude,
                }
            })
            .ToListAsync();

        var result = new GetAllWarehouseStockDto
        {
            Warehouses = warehouses,
            TotalWarehouses = warehouses.Count,
        };

        return new OkObjectResult(result);
    }
}
