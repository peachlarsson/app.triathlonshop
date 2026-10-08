namespace Peach.TriathlonShop.Database.Models;

public class Product : BaseEntity
{
    public long CategoryId { get; set; } = 1;
    public string Name { get; set; } = "Unknown";
    public decimal Price { get; set; } = 0.0m;

    public required Category Category { get; set; }
}
