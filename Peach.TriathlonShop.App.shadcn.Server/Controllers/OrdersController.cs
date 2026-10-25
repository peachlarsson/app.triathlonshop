using Microsoft.AspNetCore.Mvc;
using Peach.TriathlonShop.FunctionApp.Client;
using Peach.TriathlonShop.FunctionApp.Client.Enums.Common;
using Peach.TriathlonShop.FunctionApp.Client.Enums.Order;
using Peach.TriathlonShop.FunctionApp.Client.Models;

namespace Peach.TriathlonShop.App.shadcn.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(ITriathlonShopClient triathlonShopClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<OrdersDto>>> GetPaginated(
        SortKey sortBy = SortKey.Id,
        SortDirection sortOrder = SortDirection.Asc,
        int pageNumber = 1,
        int pageSize = 100,
        string? customerName = null)
    {
        var request = new GetOrdersRequestDto
        {
            SortKey = sortBy,
            SortDirection = sortOrder,
            PageNumber = pageNumber,
            PageSize = pageSize,
            CustomerName = customerName
        };

        var response = await triathlonShopClient.GetOrdersAsync(request);

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode, response.ErrorMessage);
        }

        if (response.Content == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var result = new PaginatedResponse<OrdersDto>
        {
            Data = response.Content.Data,
            TotalPrice = response.Content.TotalPrice,
            TotalCount = response.Content.TotalCount,
            PageNumber = response.Content.PageNumber,
            PageSize = response.Content.PageSize
        };

        return Ok(result);
    }
}

