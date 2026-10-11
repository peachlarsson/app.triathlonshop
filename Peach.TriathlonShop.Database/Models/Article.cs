namespace Peach.TriathlonShop.Database.Models;

public class Article : BaseEntity
{
    [System.ComponentModel.DataAnnotations.Schema.Column("ArticleNumber")]
    public required string Number { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("ArticleName")]
    public required string Name { get; set; }
    public required decimal UnitPrice { get; set; }
}
