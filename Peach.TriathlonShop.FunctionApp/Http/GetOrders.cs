using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Api.Contract.Enums.Common;
using Peach.TriathlonShop.Api.Contract.Enums.Order;
using Peach.TriathlonShop.Api.Contract.Models;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.FunctionApp.Utilities;

namespace Peach.TriathlonShop.FunctionApp.Http;

public class GetOrders(TriathlonShopDbContext context)
{
    [Function(nameof(GetOrders))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders")] HttpRequest req)
    {
        var queryParams = req.Query;

        if (!TryMapQuery(queryParams, out var request))
        {
            return new BadRequestResult();
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var query = context.Orders
            //.Include(o => o.Customer)
            .AsNoTracking();

        if (request.Id.HasValue)
        {
            query = query.Where(c => c.Id == request.Id.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.OrderNumber))
        {
            var term = $"%{request.OrderNumber.Trim()}%";
            query = query.Where(c => c.Number != null && EF.Functions.Like(c.Number, term));
        }

        if (!string.IsNullOrWhiteSpace(request.ArticleName))
        {
            var term = $"%{request.ArticleName.Trim()}%";
            query = query.Where(c => c.OrderRows.Any(or => or.Article != null && EF.Functions.Like(or.Article.Name, term)));
        }

        if (!string.IsNullOrWhiteSpace(request.CustomerName))
        {
            var term = $"%{request.CustomerName.Trim()}%";
            query = query.Where(c => c.Customer!.Name != null && EF.Functions.Like(c.Customer.Name, term));
        }

        query = request.SortKey switch
        {
            SortKey.OrderNumber => request.SortDirection == SortDirection.Asc 
                ? query.OrderBy(q => q.Number)
                : query.OrderByDescending(q => q.Number),
            SortKey.CustomerName => request.SortDirection == SortDirection.Asc 
                ? query.OrderBy(q => q.Customer!.Name) 
                : query.OrderByDescending(q => q.Customer!.Name),
            SortKey.OrderStatus => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.Status)
                : query.OrderByDescending(q => q.Status),
            SortKey.TotalPrice => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.OrderRows.Sum(or => or.RowPrice))
                : query.OrderByDescending(q => q.OrderRows.Sum(or => or.RowPrice)),
            SortKey.CreatedAt => request.SortDirection == SortDirection.Asc 
                ? query.OrderBy(q => q.CreatedAt) 
                : query.OrderByDescending(q => q.CreatedAt),
            SortKey.UpdatedAt => request.SortDirection == SortDirection.Asc 
                ? query.OrderBy(q => q.UpdatedAt) 
                : query.OrderByDescending(q => q.UpdatedAt),
            _ => request.SortDirection == SortDirection.Asc 
                ? query.OrderBy(q => q.Id) 
                : query.OrderByDescending(q => q.Id)
        };

        var queryDto = query.Select(c => new OrderResponseDto
        {
            Id = c.Id,
            Number = c.Number,
            Status = MapOrderStatus(c.Status),
            DestinationLocation = new LocationDto
            {
                CountryCode = c.DestinationLocation.CountryCode,
                CountryName = c.DestinationLocation.CountryName,
                City = c.DestinationLocation.City
            },
            Customer = new CustomerDto
            {
                Id = c.Customer!.Id,
                Email = c.Customer.Email,
                Name = c.Customer.Name,
                CreatedAt = c.Customer.CreatedAt,
                UpdatedAt = c.Customer.UpdatedAt
            },
            OrderRows = c.OrderRows.Select(or => new OrderRowDto
            {
                Id = or.Id,
                OrderId = or.OrderId,
                OrderNumber = or.Order!.Number,
                LineNumber = or.LineNumber,
                ArticleId = or.ArticleId,
                ArticleName = or.Article!.Name,
                UnitPrice = or.Article!.UnitPrice,
                Quantity = or.Quantity,
                RowPrice = or.RowPrice,
                CustomerId = or.Order.CustomerId,
                CustomerName = or.Order!.Customer!.Name ?? "MissingName",
                CreatedAt = or.CreatedAt,
                UpdatedAt = or.UpdatedAt
            }).ToList(),
            //Notifications
            //TransportEvents
            //Invoices
            TotalPrice = c.OrderRows.Sum(or => or.RowPrice),
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });

        var paginatedResult = await PaginationHelper.PaginateAsync(queryDto, request.PageNumber, request.PageSize);
        return new OkObjectResult(paginatedResult);
    }

    private static OrderStatus MapOrderStatus(Database.Models.Enums.OrderStatus status)
    {
        return status switch
        {
            Database.Models.Enums.OrderStatus.Created => OrderStatus.Created,
            Database.Models.Enums.OrderStatus.Confirmed => OrderStatus.Confirmed,
            Database.Models.Enums.OrderStatus.Shipped => OrderStatus.Shipped,
            Database.Models.Enums.OrderStatus.Cancelled => OrderStatus.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected order status value: {status}")
        };
    }

    private static bool TryMapQuery(IQueryCollection query, out GetOrdersRequestDto? request)
    {
        long? id = null;

        if (query.TryGetValue("id", out var idValue))
        {
            id = long.TryParse(idValue, out var result) ? result : null;
        }

        string? articleName = null;

        if (query.TryGetValue("articleName", out var an))
        {
            articleName = an;
        }

        string? customerName = null;

        if (query.TryGetValue("customerName", out var cn))
        {
            customerName = cn;
        }

        string? orderNumber = null;

        if (query.TryGetValue("orderNumber", out var on))
        {
            orderNumber = on;
        }

        var pageNumber = 1;

        if (query.TryGetValue("pageNumber", out var pn))
        {
            pageNumber = int.TryParse(pn, out var result) ? result : 1;
        }

        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        var pageSize = 100;

        if (query.TryGetValue("pageSize", out var ps))
        {
            pageSize = int.TryParse(ps, out var result) ? result : 100;
        }

        if (pageSize < 1)
        {
            pageSize = 100;
        }
        
        SortKey sortKey = SortKey.Id;

        if (query.TryGetValue("sortKey", out var sk))
        {
            sortKey = Enum.TryParse<SortKey>(sk, true, out var result) ? result : SortKey.Id;
        }

        SortDirection sortDirection = SortDirection.Asc;

        if (query.TryGetValue("sortDirection", out var sd))
        {
            sortDirection = Enum.TryParse<SortDirection>(sd, true, out var result) ? result : SortDirection.Asc;
        }

        request = new GetOrdersRequestDto
        {
            Id = id,
            CustomerName = customerName,
            OrderNumber = orderNumber,
            ArticleName = articleName,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortKey = sortKey,
            SortDirection = sortDirection,
        };

        return true;
    }
}
