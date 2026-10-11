using Microsoft.AspNetCore.Mvc;
using Peach.TriathlonShop.App.Server.Controllers.Enums.Common;
using Peach.TriathlonShop.App.Server.Controllers.Enums.Customer;
using Peach.TriathlonShop.App.Server.Controllers.Models;
using Peach.TriathlonShop.App.Server.Models;
using Peach.TriathlonShop.App.Server.Utilities;
using Peach.TriathlonShop.Database;

namespace Peach.TriathlonShop.App.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(TriathlonShopDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<CustomerDto>>> GetPaginated(
        SortKey sortBy = SortKey.Id,
        SortDirection sortOrder = SortDirection.Asc,
        int pageNumber = 1,
        int pageSize = 100,
        string? customerName = null)
    {
        var query = context.Customers
            .Select(q => new CustomerDto
            {
                Id = q.Id,
                Name = q.Name,
                Email = q.Email,
                CreatedAt = q.CreatedAt,
            })
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(customerName))
        {
            query = query.Where(q => q.Name != null && q.Name.ToLower().Contains(customerName.ToLower()));
        }

        query = sortBy switch
        {
            SortKey.Name => sortOrder == SortDirection.Asc
                ? query.OrderBy(q => q.Name)
                : query.OrderByDescending(q => q.Name),
            SortKey.Email => sortOrder == SortDirection.Asc
                ? query.OrderBy(q => q.Email)
                : query.OrderByDescending(q => q.Email),
            SortKey.CreatedAt => sortOrder == SortDirection.Asc
                ? query.OrderBy(q => q.CreatedAt)
                : query.OrderByDescending(q => q.CreatedAt),
            SortKey.UpdatedAt => sortOrder == SortDirection.Asc
                ? query.OrderBy(q => q.UpdatedAt)
                : query.OrderByDescending(q => q.UpdatedAt),
            _ => sortOrder == SortDirection.Asc
                ? query.OrderBy(q => q.Id)
                : query.OrderByDescending(q => q.Id)
        };

        var paginatedResult = await PaginationHelper.PaginateAsync(query, pageNumber, pageSize);
        return Ok(paginatedResult);
    }
}

