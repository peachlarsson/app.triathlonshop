namespace Peach.TriathlonShop.App.Server.Controllers.Models;

public class CreateOrderRequest
{
    public List<CreateOrderRowRequest> OrderRows { get; set; } = [];
    public required long CustomerId { get; set; }
}
