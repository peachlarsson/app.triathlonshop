namespace Peach.TriathlonShop.Database.Models;

public class Article : BaseEntity
{
    public required string Name { get; set; }
    public required decimal UnitPrice { get; set; }
}
