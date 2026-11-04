namespace Peach.TriathlonShop.FunctionApp.Extensions;

public static class MappingExtensions
{
    public static Api.Contract.Enums.Common.OrderStatus ToApiEnum(this Database.Models.Enums.OrderStatus status)
    {
        return status switch
        {
            Database.Models.Enums.OrderStatus.Created => Api.Contract.Enums.Common.OrderStatus.Created,
            Database.Models.Enums.OrderStatus.Confirmed => Api.Contract.Enums.Common.OrderStatus.Confirmed,
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
            Api.Contract.Enums.Common.OrderStatus.Shipped => Database.Models.Enums.OrderStatus.Shipped,
            Api.Contract.Enums.Common.OrderStatus.Cancelled => Database.Models.Enums.OrderStatus.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected order status value: {status}")
        };
    }

    public static Api.Contract.Enums.Common.InvoiceStatus MapApiInvoiceStatus(this Database.Models.Enums.InvoiceStatus status)
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

    public static Database.Models.Enums.InvoiceStatus MapDatabaseInvoiceStatus(this Api.Contract.Enums.Common.InvoiceStatus status)
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
}
