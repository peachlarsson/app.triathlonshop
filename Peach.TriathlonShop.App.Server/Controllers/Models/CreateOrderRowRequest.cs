namespace Peach.TriathlonShop.App.Server.Controllers.Models;

public class CreateOrderRowRequest
{
    public required long ArticleId { get; set; }
    public required int Quantity { get; set; }
    public required int LineNumber { get; set; }
}
