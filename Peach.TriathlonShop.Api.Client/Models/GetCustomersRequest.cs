using Peach.TriathlonShop.Api.Client.Enums.Common;
using Peach.TriathlonShop.Api.Client.Enums.Customer;

namespace Peach.TriathlonShop.Api.Client.Models;

public class GetCustomersRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 100;
    public SortKey SortKey { get; set; } = SortKey.Name;
    public SortDirection SortDirection { get; set; } = SortDirection.Asc;
    public string? CustomerName { get; set; }
}