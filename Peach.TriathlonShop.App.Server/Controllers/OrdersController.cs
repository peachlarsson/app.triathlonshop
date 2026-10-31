using Microsoft.AspNetCore.Mvc;
using Peach.TriathlonShop.Api.Client;
using Peach.TriathlonShop.Api.Contract.Enums.Common;
using Peach.TriathlonShop.Api.Contract.Enums.Order;
using Peach.TriathlonShop.Api.Contract.Models;
using Peach.TriathlonShop.Api.Contract.Models.Orders;

namespace Peach.TriathlonShop.App.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(ITriathlonShopClient triathlonShopClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<OrderResponseDto>>> GetPaginated([FromQuery] GetOrdersRequestDto request)
    {
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

        var result = new PaginatedResponse<OrderResponseDto>
        {
            Data = response.Content.Data,
            TotalPrice = response.Content.TotalPrice,
            TotalCount = response.Content.TotalCount,
            PageNumber = response.Content.PageNumber,
            PageSize = response.Content.PageSize
        };

        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        var response = await triathlonShopClient.CreateOrderAsync(request);

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
        return Ok(response.Content);
    }
}

