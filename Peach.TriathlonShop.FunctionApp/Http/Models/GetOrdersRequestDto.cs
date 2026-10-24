using Peach.TriathlonShop.FunctionApp.Http.Enums.Common;
using Peach.TriathlonShop.FunctionApp.Http.Enums.Order;

namespace Peach.TriathlonShop.FunctionApp.Http.Models;

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
