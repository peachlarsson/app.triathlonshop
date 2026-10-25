using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Api.Contract.Enums.Common;
using Peach.TriathlonShop.Api.Contract.Enums.Customer;
using Peach.TriathlonShop.Api.Contract.Models;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.FunctionApp.Utilities;

namespace Peach.TriathlonShop.FunctionApp.Http;

public class GetCustomers(TriathlonShopDbContext context)
{
    [Function(nameof(GetCustomers))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "customers")] HttpRequest req)
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

        var customers = context.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.CustomerName))
        {
            var term = $"%{request.CustomerName.Trim()}%";
            customers = customers.Where(c => c.Name != null && EF.Functions.Like(c.Name, term));
        }

        customers = request.SortKey switch
        {
            SortKey.Name => request.SortDirection == SortDirection.Asc 
                ? customers.OrderBy(c => c.Name) 
                : customers.OrderByDescending(c => c.Name),
            SortKey.Email => request.SortDirection == SortDirection.Asc 
                ? customers.OrderBy(c => c.Email) 
                : customers.OrderByDescending(c => c.Email),
            SortKey.CreatedAt => request.SortDirection == SortDirection.Asc 
                ? customers.OrderBy(c => c.CreatedAt) 
                : customers.OrderByDescending(c => c.CreatedAt),
            SortKey.UpdatedAt => request.SortDirection == SortDirection.Asc 
                ? customers.OrderBy(c => c.UpdatedAt) 
                : customers.OrderByDescending(c => c.UpdatedAt),
            _ => request.SortDirection == SortDirection.Asc 
                ? customers.OrderBy(c => c.Id) 
                : customers.OrderByDescending(c => c.Id)
        };

        var query = customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });

        var paginatedResult = await PaginationHelper.PaginateAsync(query, request.PageNumber, request.PageSize);
        return new OkObjectResult(paginatedResult);
    }

    private static bool TryMapQuery(IQueryCollection query, out GetCustomersRequest? request)
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

        SortKey sortKey = SortKey.Name;

        if (query.TryGetValue("sortKey", out var sk))
        {
            sortKey = Enum.TryParse<SortKey>(sk, true, out var result) ? result : SortKey.Name;
        }

        SortDirection sortDirection = SortDirection.Asc;

        if (query.TryGetValue("sortDirection", out var sd))
        {
            sortDirection = Enum.TryParse<SortDirection>(sd, true, out var result) ? result : SortDirection.Asc;
        }

        string? customerName = null;

        if (query.TryGetValue("customerName", out var cn))
        {
            customerName = cn.ToString();
        }

        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 100;
        }

        request = new GetCustomersRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortKey = sortKey,
            SortDirection = sortDirection,
            CustomerName = customerName,
        };

        return true;
    }
}

public class GetCustomersRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 100;
    public SortKey SortKey { get; set; } = SortKey.Name;
    public SortDirection SortDirection { get; set; } = SortDirection.Asc;
    public string? CustomerName { get; set; }
}