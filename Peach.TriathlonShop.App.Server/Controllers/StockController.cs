using Microsoft.AspNetCore.Mvc;
using Peach.TriathlonShop.Api.Client;
using Peach.TriathlonShop.Api.Contract.Models.Stock;

namespace Peach.TriathlonShop.App.Server.Controllers;

[ApiController]
[Route("api/stock")]
public class StockController(ITriathlonShopClient triathlonShopClient) : ControllerBase
{
    [HttpGet("all-warehouses")]
    public async Task<ActionResult<GetAllWarehouseStockDto>> GetAllWarehouseStockAsync()
    {
        var response = await triathlonShopClient.GetAllWarehouseStockAsync();

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode, response.ErrorMessage);
        }

        return Ok(response.Content?.Warehouses ?? []);
    }
}
