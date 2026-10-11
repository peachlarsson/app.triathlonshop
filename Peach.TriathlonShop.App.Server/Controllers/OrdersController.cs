using Microsoft.AspNetCore.Mvc;
using Peach.TriathlonShop.App.Server.Controllers.Enums.Common;
using Peach.TriathlonShop.App.Server.Controllers.Enums.Order;
using Peach.TriathlonShop.App.Server.Controllers.Models;
using Peach.TriathlonShop.App.Server.Models;
using Peach.TriathlonShop.App.Server.Utilities;
using Peach.TriathlonShop.Database;

namespace Peach.TriathlonShop.App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(TriathlonShopDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<OrderDto>>> GetPaginated(
            SortKey sortBy = SortKey.Id,
            SortDirection sortOrder = SortDirection.Asc,
            int pageNumber = 1,
            int pageSize = 100,
            string? customerName = null)
        {
            var query = context.Orders
                .Select(q => new OrderDto
                {
                    Id = q.Id,
                    Number = q.Number,
                    TotalPrice = q.TotalPrice,
                    CustomerId = q.CustomerId,
                    CustomerName = q.Customer.Name ?? "MissingName",
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                })
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                query = query.Where(q => q.CustomerName.ToLower().Contains(customerName.ToLower()));
            }

            query = sortBy switch
            {
                SortKey.Number => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.Number)
                    : query.OrderByDescending(q => q.Number),
                SortKey.TotalPrice => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.TotalPrice)
                    : query.OrderByDescending(q => q.TotalPrice),
                SortKey.CustomerId => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.CustomerId)
                    : query.OrderByDescending(q => q.CustomerId),
                SortKey.CustomerName => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.CustomerName)
                    : query.OrderByDescending(q => q.CustomerName),
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
}

