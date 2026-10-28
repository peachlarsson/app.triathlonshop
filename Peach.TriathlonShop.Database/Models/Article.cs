namespace Peach.TriathlonShop.Database.Models;

public class Article : BaseEntity
{
    public required string ArticleNumber { get; set; }
    public required string ArticleName { get; set; }
    public required decimal UnitPrice { get; set; }
}
