namespace Peach.TriathlonShop.Api.Contract.Models.Orders;

public class CreateOrderRequestDto
{
    public List<AddOrderRowDto> OrderRows { get; set; } = [];
    public required long CustomerId { get; set; }
}
