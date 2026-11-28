namespace Peach.TriathlonShop.FunctionApp.Extensions;

public static class MappingExtensions
{
    public static Api.Contract.Enums.Common.OrderStatus ToApiEnum(this Database.Models.Enums.OrderStatus status)
    {
        return status switch
        {
            Database.Models.Enums.OrderStatus.Created => Api.Contract.Enums.Common.OrderStatus.Created,
            Database.Models.Enums.OrderStatus.Confirmed => Api.Contract.Enums.Common.OrderStatus.Confirmed,
            Database.Models.Enums.OrderStatus.Dispatched => Api.Contract.Enums.Common.OrderStatus.Dispatched,
            Database.Models.Enums.OrderStatus.Shipped => Api.Contract.Enums.Common.OrderStatus.Shipped,
            Database.Models.Enums.OrderStatus.Cancelled => Api.Contract.Enums.Common.OrderStatus.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected order status value: {status}")
        };
    }

    public static Database.Models.Enums.OrderStatus ToDatabaseEnum(this Api.Contract.Enums.Common.OrderStatus status)
    {
        return status switch
        {
            Api.Contract.Enums.Common.OrderStatus.Created => Database.Models.Enums.OrderStatus.Created,
            Api.Contract.Enums.Common.OrderStatus.Confirmed => Database.Models.Enums.OrderStatus.Confirmed,
            Api.Contract.Enums.Common.OrderStatus.Dispatched => Database.Models.Enums.OrderStatus.Dispatched,
            Api.Contract.Enums.Common.OrderStatus.Shipped => Database.Models.Enums.OrderStatus.Shipped,
            Api.Contract.Enums.Common.OrderStatus.Cancelled => Database.Models.Enums.OrderStatus.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected order status value: {status}")
        };
    }

    public static List<Database.Models.Enums.OrderStatus> ToDatabaseEnum(this List<Api.Contract.Enums.Common.OrderStatus> statuses)
    {
        return statuses.Select(status => status.ToDatabaseEnum()).ToList();
    }

    public static Api.Contract.Enums.Common.InvoiceStatus ToApiEnum(this Database.Models.Enums.InvoiceStatus status)
    {
        return status switch
        {
            Database.Models.Enums.InvoiceStatus.Created => Api.Contract.Enums.Common.InvoiceStatus.Created,
            Database.Models.Enums.InvoiceStatus.Sent => Api.Contract.Enums.Common.InvoiceStatus.Sent,
            Database.Models.Enums.InvoiceStatus.Paid => Api.Contract.Enums.Common.InvoiceStatus.Paid,
            Database.Models.Enums.InvoiceStatus.Cancelled => Api.Contract.Enums.Common.InvoiceStatus.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected invoice status value: {status}")
        };
    }

    public static Database.Models.Enums.InvoiceStatus ToDatabaseEnum(this Api.Contract.Enums.Common.InvoiceStatus status)
    {
        return status switch
        {
            Api.Contract.Enums.Common.InvoiceStatus.Created => Database.Models.Enums.InvoiceStatus.Created,
            Api.Contract.Enums.Common.InvoiceStatus.Sent => Database.Models.Enums.InvoiceStatus.Sent,
            Api.Contract.Enums.Common.InvoiceStatus.Paid => Database.Models.Enums.InvoiceStatus.Paid,
            Api.Contract.Enums.Common.InvoiceStatus.Cancelled => Database.Models.Enums.InvoiceStatus.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected invoice status value: {status}")
        };
    }

    public static Business.Enums.Common.SortDirection ToBusinessEnum(this Api.Contract.Enums.Common.SortDirection sortDirection)
    {
        return sortDirection switch
        {
            Api.Contract.Enums.Common.SortDirection.Asc => Business.Enums.Common.SortDirection.Asc,
            Api.Contract.Enums.Common.SortDirection.Desc => Business.Enums.Common.SortDirection.Desc,
            _ => throw new ArgumentOutOfRangeException(nameof(sortDirection), $"Not expected sort direction value: {sortDirection}")
        };
    }
    public static Business.Enums.Order.SortKey ToBusinessEnum(this Api.Contract.Enums.Order.SortKey sortKey)
    {
        return sortKey switch
        {
            Api.Contract.Enums.Order.SortKey.Id => Business.Enums.Order.SortKey.Id,
            Api.Contract.Enums.Order.SortKey.CustomerName => Business.Enums.Order.SortKey.CustomerName,
            Api.Contract.Enums.Order.SortKey.OrderNumber => Business.Enums.Order.SortKey.OrderNumber,
            _ => throw new ArgumentOutOfRangeException(nameof(sortKey), $"Not expected sort key value: {sortKey}")
        };
    }

    public static Business.Queries.GetOrdersQuery Map(this Api.Contract.Models.GetOrdersRequestDto request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return new()
        {
            Id = request.Id,
            CustomerName = request.CustomerName,
            OrderStatuses = request.OrderStatuses?.ToDatabaseEnum(),
            OrderNumber = request.OrderNumber,
            ArticleName = request.ArticleName,
            IncludeOrderRows = request.IncludeOrderRows,
            IncludeCustomer = request.IncludeCustomer,
            DestinationCountry = request.DestinationCountry,
            DestinationCity = request.DestinationCity,
            DestinationLocation = request.DestinationLocation,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            SortKey = request.SortKey.ToBusinessEnum(),
            SortDirection = request.SortDirection.ToBusinessEnum()
        };
    }
}
