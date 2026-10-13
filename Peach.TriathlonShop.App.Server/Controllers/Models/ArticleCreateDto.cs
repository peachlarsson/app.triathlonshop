namespace Peach.TriathlonShop.App.Server.Controllers.Models;

public class ArticleCreateDto
{
    public required long Id { get; set; }
    public required int Quantity { get; set; }
}