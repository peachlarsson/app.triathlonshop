using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.FunctionApp.Http.Enums.Common;
using Peach.TriathlonShop.FunctionApp.Http.Enums.Order;
using Peach.TriathlonShop.FunctionApp.Http.Models;
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

        var query = context.Orders.AsNoTracking();

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
            Status = c.Status,
            Customer = new OrderCustomerDto
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

    private static bool TryMapQuery(IQueryCollection query, out GetOrdersRequestDto? request)
    {
        var pageNumber = 1;

        if (query.TryGetValue("pageNumber", out var pn))
        {
            pageNumber = int.TryParse(pn, out var result) ? result : 1;
        }

        var pageSize = 100;

        if (query.TryGetValue("pageSize", out var ps))
        {
            pageSize = int.TryParse(ps, out var result) ? result : 100;
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

        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 100;
        }

        request = new GetOrdersRequestDto
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortKey = sortKey,
            SortDirection = sortDirection,
        };

        return true;
    }
}
