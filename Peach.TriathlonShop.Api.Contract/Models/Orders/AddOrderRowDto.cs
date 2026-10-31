namespace Peach.TriathlonShop.Api.Contract.Models.Orders;

public class AddOrderRowDto
{
    public long ArticleId { get; set; }
    public int Quantity { get; set; }
}