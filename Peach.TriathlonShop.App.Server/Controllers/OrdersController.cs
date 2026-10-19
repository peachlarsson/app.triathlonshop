using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.App.Server.Controllers.Enums.Common;
using Peach.TriathlonShop.App.Server.Controllers.Enums.Order;
using Peach.TriathlonShop.App.Server.Controllers.Models;
using Peach.TriathlonShop.App.Server.Models;
using Peach.TriathlonShop.App.Server.Utilities;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;
using Peach.TriathlonShop.Database.Models.Enums;
using Peach.TriathlonShop.FunctionApp.Client;
using System.Net;

namespace Peach.TriathlonShop.App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(TriathlonShopDbContext context, ITriathlonShopClient triathlonShopClient) : ControllerBase
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
                    Status = q.Status,
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
                    NotificationHistory = context.Notifications
                        .Where(notification => notification.OrderId == q.Id)
                        .OrderByDescending(notification => notification.CreatedAt)
                        .Select(notification => new NotificationHistoryDto
                        {
                            Id = notification.Id,
                            OrderStatus = notification.OrderStatus.ToString(),
                            Destination = notification.Destination,
                            CreatedAt = notification.CreatedAt,
                            UpdatedAt = notification.UpdatedAt,
                        }).ToList(),
                    TransportHistory = context.TransportOrders
                        .Where(transport => transport.OrderId == q.Id)
                        .OrderByDescending(transport => transport.CreatedAt)
                        .Select(transport => new TransportHistoryDto
                        {
                            Id = transport.Id,
                            Status = transport.Status.ToString(),
                            Carrier = transport.Carrier,
                            ConsignmentNumber = transport.ConsignmentNumber,
                            TrackingUrl = transport.TrackingUrl,
                            Destination = transport.Destination,
                            ShippedAt = transport.ShippedAt,
                            EstimatedDeliveryDate = transport.EstimatedDeliveryDate,
                            CreatedAt = transport.CreatedAt,
                            UpdatedAt = transport.UpdatedAt,
                        }).ToList(),
                    InvoiceHistory = context.Invoices
                        .Where(invoice => invoice.OrderId == q.Id)
                        .OrderByDescending(invoice => invoice.CreatedAt)
                        .Select(invoice => new InvoiceHistoryDto
                        {
                            Id = invoice.Id,
                            Status = invoice.Status.ToString(),
                            CreatedAt = invoice.CreatedAt,
                            UpdatedAt = invoice.UpdatedAt,
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
            string? customerName = null,
            string? articleName = null)
        {
            var query = context.Orders
                .Select(q => new OrderDto
                {
                    Id = q.Id,
                    Number = q.Number,
                    Status = q.Status,
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

            if (!string.IsNullOrWhiteSpace(articleName))
            {
                query = query.Where(q => q.Items.Any(item => item.ArticleName.ToLower().Contains(articleName.ToLower())));
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
                Status = OrderStatus.Created,
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
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                CustomerId = order.CustomerId,
                CustomerName = customer.Name ?? "MissingName",
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
            };

            return CreatedAtAction(nameof(Get), new { id = order.Id }, createdOrder);
        }

        [HttpPost("confirm/{orderId}")]
        public async Task<ActionResult> OrderConfirm(long orderId)
        {
            var response = await triathlonShopClient.SetOrderConfirm(orderId);

            if (response == null)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to confirm order. Response is null.");
            }

            if (response.IsSuccess)
            {
                return Ok();
            }

                return StatusCode(response.StatusCode, $"Failed to confirm order. StatusCode: {response.StatusCode}, ErrorMessage: {response.ErrorMessage}");
            }

        [HttpPost("ship/{orderId}")]
        public async Task<ActionResult> OrderShip(long orderId)
        {
            var response = await triathlonShopClient.SetOrderShipped(orderId);

            if (response == null)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to ship order. Response is null.");
            }

            if (response.IsSuccess)
            {
            return Ok();
        }

            return StatusCode(response.StatusCode, $"Failed to ship order. StatusCode: {response.StatusCode}, ErrorMessage: {response.ErrorMessage}");
        }

        [HttpGet("negotiation")]
        public async Task<SignalRConnectionInfo> GetNegotiationAsync()
        {
            var response = await triathlonShopClient.GetNegotiationAsync(hubName: "Orders");

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
}
