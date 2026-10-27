namespace Peach.TriathlonShop.Api.Contract.Models.Stock;

public class ArticleDto
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string Number { get; set; }
    public required decimal UnitPrice { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
