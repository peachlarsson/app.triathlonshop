using Peach.TriathlonShop.Api.Client.Enums.Common;
using Peach.TriathlonShop.Api.Client.Enums.Order;

namespace Peach.TriathlonShop.Api.Client.Models;

public class GetOrdersRequestDto
{
    public string? OrderNumber { get; set; }
    public string? CustomerName { get; set; }
    public string? ArticleName { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 100;
    public SortKey SortKey { get; set; } = SortKey.Id;
    public SortDirection SortDirection { get; set; } = SortDirection.Asc;
}