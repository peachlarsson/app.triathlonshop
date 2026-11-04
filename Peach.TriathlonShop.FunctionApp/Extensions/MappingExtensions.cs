namespace Peach.TriathlonShop.FunctionApp.Extensions;

public static class MappingExtensions
{
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
