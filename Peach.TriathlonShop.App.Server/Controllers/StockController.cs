using Microsoft.AspNetCore.Mvc;
using Peach.TriathlonShop.Api.Client;
using Peach.TriathlonShop.Api.Contract.Enums.Article;
using Peach.TriathlonShop.Api.Contract.Enums.Common;
using Peach.TriathlonShop.Api.Contract.Models;
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

    [HttpGet("articles")]
    public async Task<ActionResult<PaginatedResponse<ArticleDto>>> GetArticlesAsync(
        [FromQuery] string? articleName = null,
        [FromQuery] SortKey sortKey = SortKey.ArticleName,
        [FromQuery] SortDirection sortDirection = SortDirection.Asc,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 100)
    {
        var response = await triathlonShopClient.GetArticlesAsync(new GetArticlesRequestDto
        {
            ArticleName = articleName,
            SortKey = sortKey,
            SortDirection = sortDirection,
            PageNumber = pageNumber,
            PageSize = pageSize
        });

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode, response.ErrorMessage);
        }
        
        return Ok(response.Content);
    }
}
