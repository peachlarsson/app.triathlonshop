namespace Peach.TriathlonShop.Api.Contract.Models.Stock;

public class GetAllWarehouseStockDto
{
    public List<WarehouseStockDto> Warehouses { get; set; } = [];
    public int TotalWarehouses { get; set; }
}

public class WarehouseStockDto
{
    public long WarehouseId { get; set; }
    public required string WarehouseName { get; set; }
    public required LocationDto Location { get; set; }
    public List<ArticleStockDto> Articles { get; set; } = [];
}

public class ArticleStockDto
{
    public long ArticleId { get; set; }
    public required string ArticleName { get; set; }
    public required string ArticleNumber { get; set; }
    public int Quantity { get; set; }
}