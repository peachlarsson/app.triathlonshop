namespace Peach.TriathlonShop.Api.Contract.Models.Stock;

public class WarehouseOverviewDto
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public int? TotalArticles { get; set; }
    public int? InStock { get; set; }
    public int? OutOfStock { get; set; }
    public int? AwaitingDispatch { get; set; }
    public int? Shipped { get; set; }
}