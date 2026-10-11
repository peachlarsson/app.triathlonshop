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
            string? articleName = null)
        {
            var query = context.OrderRows
                .Select(q => new OrderRowDto
                {
                    Id = q.Id,
                    OrderId = q.OrderId,
                    OrderNumber = q.Order.OrderNumber,
                    LineNumber = q.LineNumber,
                    ArticleId = q.ArticleId,
                    ArticleName = q.Article.Name,
                    UnitPrice = q.Article.UnitPrice,
                    Quantity = q.Quantity,
                    RowPrice = q.RowPrice,
                    CustomerId = q.Order.CustomerId,
                    CustomerName = q.Order.Customer.Name ?? "MissingName",
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                })
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(articleName))
            {
                query = query.Where(q => q.ArticleName.ToLower().Contains(articleName.ToLower()));
            }

            query = sortBy switch
            {
                SortKey.OrderNumber => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.OrderNumber)
                    : query.OrderByDescending(q => q.OrderNumber),
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
            return Ok(paginatedResult);
        }
    }
}

