using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Api.Contract.Enums.Common;
using Peach.TriathlonShop.Api.Contract.Enums.Order;
using Peach.TriathlonShop.Api.Contract.Models;
using Peach.TriathlonShop.Api.Contract.Models.Orders;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.FunctionApp.Extensions;
using Peach.TriathlonShop.FunctionApp.Utilities;

namespace Peach.TriathlonShop.FunctionApp.Http.Orders;

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

        if (request.OrderStatuses is { Count: > 0 })
        {
            var statuses = request.OrderStatuses
                .Select(s => s.ToDatabaseEnum())
                .ToList();

            query = query.Where(c => statuses.Contains(c.Status));
        }

        if (!string.IsNullOrWhiteSpace(request.ArticleName))
        {
            var term = $"%{request.ArticleName.Trim()}%";
            query = query.Where(c => c.OrderRows.Any(or => or.Article != null && EF.Functions.Like(or.Article.ArticleName, term)));
        }

        if (!string.IsNullOrWhiteSpace(request.CustomerName))
        {
            var term = $"%{request.CustomerName.Trim()}%";
            query = query.Where(c => c.Customer!.Name != null && EF.Functions.Like(c.Customer.Name, term));
        }

        if (!string.IsNullOrWhiteSpace(request.DestinationCountry))
        {
            var term = $"%{request.DestinationCountry.Trim()}%";
            query = query.Where(c => c.DestinationLocation.Country.Name != null && EF.Functions.Like(c.DestinationLocation.Country.Name, term));
        }

        if (!string.IsNullOrWhiteSpace(request.DestinationCity))
        {
            var term = $"%{request.DestinationCity.Trim()}%";
            query = query.Where(c => c.DestinationLocation.City.Name != null && EF.Functions.Like(c.DestinationLocation.City.Name, term));
        }

        
        if (!string.IsNullOrWhiteSpace(request.DestinationLocation))
        {
            var term = $"%{request.DestinationLocation.Trim()}%";
            query = query.Where(c => c.DestinationLocation.City.Name != null && EF.Functions.Like(c.DestinationLocation.City.Name, term) ||
                                     c.DestinationLocation.Country.Name != null && EF.Functions.Like(c.DestinationLocation.Country.Name, term));
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
            SortKey.TransportStatus => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.TransportOrder!.Status)
                : query.OrderByDescending(q => q.TransportOrder!.Status),
            SortKey.EstimatedDeliveryDate => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.TransportOrder != null ? q.TransportOrder.EstimatedDeliveryDate : null)
                : query.OrderByDescending(q => q.TransportOrder != null ? q.TransportOrder.EstimatedDeliveryDate : null),
            SortKey.PlacedAt => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.PlacedAt)
                : query.OrderByDescending(q => q.PlacedAt),
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
            OrderNumber = c.Number,
            Status = c.Status.ToApiEnum(),
            DestinationLocation = new LocationDto
            {
                CountryCode = c.DestinationLocation.Country.Code,
                CountryName = c.DestinationLocation.Country.Name,
                CityName = c.DestinationLocation.City.Name,
            },
            Customer = !request.IncludeCustomer ? null : new CustomerDto
            {
                Id = c.Customer!.Id,
                Email = c.Customer.Email,
                Name = c.Customer.Name,
                CreatedAt = c.Customer.CreatedAt,
                UpdatedAt = c.Customer.UpdatedAt
            },
            OrderRows = !request.IncludeOrderRows ? new() : c.OrderRows.Select(or => new OrderRowDto
            {
                Id = or.Id,
                OrderId = or.OrderId,
                OrderNumber = or.Order!.Number,
                LineNumber = or.LineNumber,
                ArticleId = or.ArticleId,
                ArticleName = or.Article!.ArticleName,
                UnitPrice = or.Article!.UnitPrice,
                Quantity = or.Quantity,
                RowPrice = or.RowPrice,
                CustomerId = or.Order.CustomerId,
                CustomerName = or.Order!.Customer!.Name ?? "MissingName",
                PlacedAt = or.PlacedAt,
                CreatedAt = or.CreatedAt,
                UpdatedAt = or.UpdatedAt
            }).ToList(),
            Notifications = c.Notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                OrderId = n.OrderId,
                OrderStatus = n.OrderStatus.ToApiEnum(),
                Destination = n.Destination,
                CreatedAt = n.CreatedAt
            }).ToList(),
            TransportOrder = c.TransportOrder == null ? null : new TransportOrderDto
            {
                Id = c.TransportOrder.Id,
                ConsignmentNumber = c.TransportOrder.ConsignmentNumber,
                TransportOrderStatus = c.TransportOrder.Status.ToString(),
                Carrier = c.TransportOrder.Carrier,
                Destination = c.TransportOrder.Destination,
                ShippedAt = c.TransportOrder.ShippedAt,
                EstimatedDeliveryDate = c.TransportOrder.EstimatedDeliveryDate,
            },
            //TransportEvents
            Invoices = c.Invoices.Select(i => new InvoiceDto
            {
                Id = i.Id,
                OrderId = i.OrderId,
                Status = i.Status.ToApiEnum(),
                CreatedAt = i.CreatedAt
            }).ToList(),
            TotalPrice = c.OrderRows.Sum(or => or.RowPrice),
            PlacedAt = c.PlacedAt,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });

        var paginatedResult = await PaginationHelper.PaginateAsync(queryDto, request.PageNumber, request.PageSize);
        return new OkObjectResult(paginatedResult);
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

        List<OrderStatus>? orderStatuses = null;

        if (query.TryGetValue("orderStatus", out var orderStatusValues))
        {
            var parsedStatuses = new List<OrderStatus>();

            foreach (var rawValue in orderStatusValues)
            {
                if (string.IsNullOrWhiteSpace(rawValue))
                {
                    continue;
                }

                if (Enum.TryParse<OrderStatus>(rawValue.ToString().Trim(), true, out var statusEnum))
                {
                    parsedStatuses.Add(statusEnum);
                }
            }

            orderStatuses = parsedStatuses.Count > 0
                ? parsedStatuses.Distinct().ToList()
                : null;
        }

        var includeOrderRows = query.TryGetValue("includeOrderRows", out var ior) && bool.TryParse(ior, out var iorResult) && iorResult;
        var includeCustomer = query.TryGetValue("includeCustomer", out var ic) && bool.TryParse(ic, out var icResult) && icResult;

        var destinationCountry = query.TryGetValue("destinationCountry", out var dc) ? dc.ToString() : null;
        var destinationCity = query.TryGetValue("destinationCity", out var dci) ? dci.ToString() : null;
        var destinationLocation = query.TryGetValue("destinationLocation", out var dl) ? dl.ToString() : null;

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
            OrderStatuses = orderStatuses,
            OrderNumber = orderNumber,
            ArticleName = articleName,
            IncludeOrderRows = includeOrderRows,
            IncludeCustomer = includeCustomer,
            DestinationCountry = destinationCountry,
            DestinationCity = destinationCity,
            DestinationLocation = destinationLocation,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortKey = sortKey,
            SortDirection = sortDirection,
        };

        return true;
    }
}
