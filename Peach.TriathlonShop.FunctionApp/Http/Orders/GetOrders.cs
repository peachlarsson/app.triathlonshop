using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Peach.TriathlonShop.Api.Contract.Enums.Common;
using Peach.TriathlonShop.Api.Contract.Enums.Order;
using Peach.TriathlonShop.Api.Contract.Models;
using Peach.TriathlonShop.FunctionApp.Business;
using Peach.TriathlonShop.FunctionApp.Extensions;

namespace Peach.TriathlonShop.FunctionApp.Http.Orders;

public class GetOrders(OrdersHandler ordersHandler)
{
    [Function(nameof(GetOrders))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders")] HttpRequest req)
    {
        var queryParams = req.Query;

        if (!TryMapQuery(queryParams, out var request))
        {
            return new BadRequestResult();
        }

        var result = await ordersHandler.GetOrdersAsync(request.Map());

        return new OkObjectResult(result);
    }

    private static bool TryMapQuery(IQueryCollection query, out GetOrdersRequestDto request)
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
