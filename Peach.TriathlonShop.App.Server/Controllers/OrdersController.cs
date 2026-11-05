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

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
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

    [HttpPost("{orderid}/confirm")]
    public async Task<IActionResult> ConfirmOrder(long orderId)
    {
        var response = await triathlonShopClient.SetOrderConfirm(orderId);

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode, response.ErrorMessage);
        }
        
        return Ok();
    }

    [HttpPost("{orderid}/ship")]
    public async Task<IActionResult> ShipOrder(long orderId)
    {
        var response = await triathlonShopClient.SetOrderShipped(orderId);

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode, response.ErrorMessage);
        }

        return Ok();
    }

    [HttpGet("negotiation/orders")]
    public async Task<SignalRConnectionInfo> GetNegotiationAsync()
    {
        var response = await triathlonShopClient.GetNegotiationAsync("orders");

        if (!response.IsSuccess || response.Content is null)
        {
            throw new Exception($"Failed to get SignalR negotiation info. StatusCode: {response.StatusCode}, ErrorMessage: {response.ErrorMessage}");
        }

        return new SignalRConnectionInfo
        {
            Url = response.Content.Url,
            AccessToken = response.Content.AccessToken,
        };
    }
}
