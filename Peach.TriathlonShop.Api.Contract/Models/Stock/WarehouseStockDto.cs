namespace Peach.TriathlonShop.Api.Contract.Models.Stock;

public class WarehouseStockDto
{
    public long WarehouseId { get; set; }
    public List<ArticleStockDto> Articles { get; set; } = [];
}

public class ArticleStockDto
{
    public long ArticleId { get; set; }
    public int Quantity { get; set; }
}