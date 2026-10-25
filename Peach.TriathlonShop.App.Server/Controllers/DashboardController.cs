using Microsoft.AspNetCore.Mvc;
using Peach.TriathlonShop.Api.Client;
using Peach.TriathlonShop.Api.Contract.Developer.Models;
using Peach.TriathlonShop.Api.Contract.Models;

namespace Peach.TriathlonShop.App.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(ITriathlonShopClient triathlonShopClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardResponseModel>> GetDashboardData()
    {
        var response = await triathlonShopClient.GetDashboardDataAsync();

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

    [HttpPost("developer/create-random-orders")]
    public async Task<ActionResult<string>> CreateRandomOrders([FromBody] CreateRandomOrdersModel request)
    {
        var response = await triathlonShopClient.CreateRandomOrdersAsync(request);

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

