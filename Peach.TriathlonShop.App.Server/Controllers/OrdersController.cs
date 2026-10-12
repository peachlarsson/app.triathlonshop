using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.App.Server.Controllers.Enums.Common;
using Peach.TriathlonShop.App.Server.Controllers.Enums.Order;
using Peach.TriathlonShop.App.Server.Controllers.Models;
using Peach.TriathlonShop.App.Server.Models;
using Peach.TriathlonShop.App.Server.Utilities;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(TriathlonShopDbContext context) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetById(
            int id,
            SortKey sortBy = SortKey.Id,
            SortDirection sortOrder = SortDirection.Asc)
        {
            var query = context.Orders
                .Where(q => q.Id == id)
                .Select(q => new OrderDto
                {
                    Id = q.Id,
                    Number = q.Number,
                    Items = q.OrderRows.Select(row => new OrderRowDto
                    {
                        OrderId = q.Id,
                        OrderNumber = row.Order!.Number,
                        LineNumber = row.LineNumber,
                        ArticleId = row.ArticleId,
                        ArticleName = row.Article!.Name,
                        UnitPrice = row.Article!.UnitPrice,
                        Quantity = row.Quantity,
                        RowPrice = row.RowPrice,
                        CustomerId = row.Order!.CustomerId,
                        CustomerName = row.Order!.Customer!.Name ?? "MissingName",
                        CreatedAt = row.CreatedAt,
                        UpdatedAt = row.UpdatedAt,
                    }).ToList(),
                    TotalPrice = q.TotalPrice,
                    CustomerId = q.CustomerId,
                    CustomerName = q.Customer != null && q.Customer.Name != null ? q.Customer.Name : "MissingName",
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                })
                .AsQueryable();

            switch (sortBy)
            {
                case SortKey.Number:
                    query = sortOrder == SortDirection.Asc
                        ? query.OrderBy(q => q.Number)
                        : query.OrderByDescending(q => q.Number);
                    break;
                case SortKey.CustomerId:
                    query = sortOrder == SortDirection.Asc
                        ? query.OrderBy(q => q.CustomerId)
                        : query.OrderByDescending(q => q.CustomerId);
                    break;
                case SortKey.CustomerName:
                    query = sortOrder == SortDirection.Asc
                        ? query.OrderBy(q => q.CustomerName)
                        : query.OrderByDescending(q => q.CustomerName);
                    break;
                case SortKey.CreatedAt:
                    query = sortOrder == SortDirection.Asc
                        ? query.OrderBy(q => q.CreatedAt)
                        : query.OrderByDescending(q => q.CreatedAt);
                    break;
                case SortKey.UpdatedAt:
                    query = sortOrder == SortDirection.Asc
                        ? query.OrderBy(q => q.UpdatedAt)
                        : query.OrderByDescending(q => q.UpdatedAt);
                    break;
                default:
                    query = sortOrder == SortDirection.Asc
                        ? query.OrderBy(q => q.Id)
                        : query.OrderByDescending(q => q.Id);
                    break;
            }

            var order = await query.FirstOrDefaultAsync();

            if (order is null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<OrderDto>>> Get(
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
                    Items = q.OrderRows.Select(row => new OrderRowDto
                    {
                        OrderId = q.Id,
                        OrderNumber = row.Order!.Number,
                        LineNumber = row.LineNumber,
                        ArticleId = row.ArticleId,
                        ArticleName = row.Article!.Name,
                        UnitPrice = row.Article!.UnitPrice,
                        Quantity = row.Quantity,
                        RowPrice = row.RowPrice,
                        CustomerId = row.Order!.CustomerId,
                        CustomerName = row.Order!.Customer!.Name ?? "MissingName",
                        CreatedAt = row.CreatedAt,
                        UpdatedAt = row.UpdatedAt,
                    }).ToList(),
                    CustomerId = q.CustomerId,
                    CustomerName = q.Customer != null && q.Customer.Name != null ? q.Customer.Name : "MissingName",
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

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var customer = await context.Customers.FindAsync(request.CustomerId);

            if (customer is null)
                return BadRequest("Customer does not exist.");

            if (request.OrderRows.Count == 0)
                return BadRequest("At least one order row is required.");

            if (request.OrderRows.Any(row => row.Quantity <= 0))
                return BadRequest("Order row quantity must be greater than 0.");

            var articleIds = request.OrderRows.Select(row => row.ArticleId).Distinct().ToList();

            var articles = await context.Articles
                .Where(article => articleIds.Contains(article.Id))
                .ToDictionaryAsync(article => article.Id);

            if (articles.Count != articleIds.Count)
                return BadRequest("One or more selected articles do not exist.");

            var orderRows = request.OrderRows
                .Select(row => new
                {
                    Row = row,
                    Article = articles[row.ArticleId]
                })
                .ToList();

            var order = new Order
            {
                Id = 0,
                Number = "ORD-" + Guid.NewGuid(),
                TotalPrice = orderRows.Sum(row => row.Article.UnitPrice * row.Row.Quantity),
                CustomerId = request.CustomerId,
                Customer = customer,
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var orderRowsToAdd = request.OrderRows.Select(row => new OrderRow
            {
                OrderId = order.Id,
                Order = order,
                LineNumber = row.LineNumber,
                ArticleId = row.ArticleId,
                Article = articles[row.ArticleId],
                Quantity = row.Quantity,
                RowPrice = row.Quantity * articles[row.ArticleId].UnitPrice,
            }).ToList();

            context.OrderRows.AddRange(orderRowsToAdd);
            await context.SaveChangesAsync();

            var createdOrder = new OrderDto
            {
                Id = order.Id,
                Number = order.Number,
                TotalPrice = order.TotalPrice,
                CustomerId = order.CustomerId,
                CustomerName = customer.Name ?? "MissingName",
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
            };

            return CreatedAtAction(nameof(Get), new { id = order.Id }, createdOrder);
        }
    }
}

