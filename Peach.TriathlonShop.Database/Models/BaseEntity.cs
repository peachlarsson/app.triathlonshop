namespace Peach.TriathlonShop.Database.Models;

public abstract class BaseEntity
{
    public required long Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}