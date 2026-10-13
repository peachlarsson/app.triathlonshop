using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.App.Server.Controllers.Enums.Common;
using Peach.TriathlonShop.App.Server.Controllers.Enums.OrderRow;
using Peach.TriathlonShop.App.Server.Controllers.Models;
using Peach.TriathlonShop.App.Server.Models;
using Peach.TriathlonShop.App.Server.Utilities;
using Peach.TriathlonShop.Database;

namespace Peach.TriathlonShop.App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderRowsController(TriathlonShopDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<OrderRowDto>>> Get(
            SortKey sortBy = SortKey.Id,
            SortDirection sortOrder = SortDirection.Asc,
            int pageNumber = 1,
            int pageSize = 100,
            string? articleName = null,
            long? orderId = null)
        {
            var query = context.OrderRows
                .Select(q => new OrderRowDto
                {
                    Id = q.Id,
                    OrderId = q.OrderId,
                    OrderNumber = q.Order!.Number,
                    LineNumber = q.LineNumber,
                    ArticleId = q.ArticleId,
                    ArticleName = q.Article!.Name,
                    UnitPrice = q.Article!.UnitPrice,
                    Quantity = q.Quantity,
                    RowPrice = q.RowPrice,
                    CustomerId = q.Order.CustomerId,
                    CustomerName = q.Order!.Customer!.Name ?? "MissingName",
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                })
                .AsQueryable();

            if (orderId.HasValue)
            {
                query = query.Where(q => q.OrderId == orderId.Value);
            }

            if (!string.IsNullOrWhiteSpace(articleName))
            {
                query = query.Where(q => q.ArticleName.ToLower().Contains(articleName.ToLower()));
            }

            query = sortBy switch
            {
                SortKey.OrderId => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.OrderId)
                    : query.OrderByDescending(q => q.OrderId),
                SortKey.OrderNumber => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.OrderNumber)
                    : query.OrderByDescending(q => q.OrderNumber),
                SortKey.LineNumber => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.LineNumber)
                    : query.OrderByDescending(q => q.LineNumber),
                SortKey.ArticleName => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.ArticleName)
                    : query.OrderByDescending(q => q.ArticleName),
                SortKey.UnitPrice => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.UnitPrice)
                    : query.OrderByDescending(q => q.UnitPrice),
                SortKey.Quantity => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.Quantity)
                    : query.OrderByDescending(q => q.Quantity),
                SortKey.RowPrice => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.RowPrice)
                    : query.OrderByDescending(q => q.RowPrice),
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
            paginatedResult.TotalPrice = await query.SumAsync(q => q.RowPrice);

            return Ok(paginatedResult);
        }

        [HttpPost("create/{orderId}")]
        public async Task<IActionResult> CreateOrderRow(long orderId, OrderRowCreateDto orderRowCreateDto)
        {
            var order = await context.Orders
                .Include(o => o.OrderRows)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            foreach (var item in orderRowCreateDto.Articles)
            {
                var article = await context.Articles.FindAsync(item.Id);

                if (article == null)
                {
                    return NotFound($"Article with ID {item.Id} not found.");
                }

                var orderRow = new Database.Models.OrderRow
                {
                    OrderId = orderId,
                    LineNumber = order.OrderRows.Count + 1,
                    ArticleId = item.Id,
                    Quantity = item.Quantity,
                    RowPrice = article.UnitPrice * item.Quantity
                };

                context.OrderRows.Add(orderRow);
            }

            order.TotalPrice = order.OrderRows.Sum(or => or.RowPrice);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { orderId }, orderRowCreateDto);
        }
    }
}