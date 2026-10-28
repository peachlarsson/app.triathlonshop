using Microsoft.AspNetCore.Mvc;
using Peach.TriathlonShop.Api.Client;
using Peach.TriathlonShop.Api.Contract.Enums.Article;
using Peach.TriathlonShop.Api.Contract.Enums.Common;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController(ITriathlonShopClient triathlonShopClient) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetPaginated(
            [FromQuery] string? articleName = null,
            [FromQuery] SortKey sortKey = SortKey.Id,
            [FromQuery] SortDirection sortDirection = SortDirection.Asc,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100)
        {
            var response = await triathlonShopClient.GetArticlesAsync(new Api.Contract.Models.Stock.GetArticlesRequestDto
            {
                ArticleName = articleName,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortKey,
                SortOrder = sortDirection,
            });

            if (!response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response.ErrorMessage);
            }

            return Ok(response.Content);
        }
    }
}
