using Microsoft.AspNetCore.Mvc;
using Peach.TriathlonShop.Api.Client;
using Peach.TriathlonShop.Api.Client.Enums.Common;
using Peach.TriathlonShop.Api.Client.Enums.Customer;
using Peach.TriathlonShop.Api.Client.Models;

namespace Peach.TriathlonShop.App.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(ITriathlonShopClient triathlonShopClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<CustomerDto>>> GetPaginated(
        SortKey sortBy = SortKey.Id,
        SortDirection sortOrder = SortDirection.Asc,
        int pageNumber = 1,
        int pageSize = 100,
        string? customerName = null)
    {
        var request = new GetCustomersRequest
        {
            SortKey = sortBy,
            SortDirection = sortOrder,
            PageNumber = pageNumber,
            PageSize = pageSize,
            CustomerName = customerName
        };

        var response = await triathlonShopClient.GetCustomerAsync(request);

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

        var result = new PaginatedResponse<CustomerDto>
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

