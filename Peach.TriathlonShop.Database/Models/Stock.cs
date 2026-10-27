namespace Peach.TriathlonShop.Database.Models;

public class Stock : BaseEntity
{
    public required long ArticleId { get; set; }
    public required int Quantity { get; set; }
    public required long WarehouseId { get; set; }

    public required Article Article { get; set; }
    public required Warehouse Warehouse { get; set; }
}
