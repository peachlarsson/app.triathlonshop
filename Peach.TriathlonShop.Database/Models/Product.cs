namespace Peach.TriathlonShop.Database.Models;

public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required decimal UnitPrice { get; set; }
}
