namespace Peach.TriathlonShop.App.Server.Controllers.Models;

public class OrderRowCreateDto
{
    public List<ArticleCreateDto> Articles { get; set; } = [];
}
