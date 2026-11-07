namespace Peach.TriathlonShop.Database.Models;

public class Stock : BaseEntity
{
    public required long ArticleId { get; set; }
    public required int Quantity { get; set; }
    public required long WarehouseId { get; set; }

    public Article Article { get; set; } = default!;
    public Warehouse Warehouse { get; set; } = default!;
}
