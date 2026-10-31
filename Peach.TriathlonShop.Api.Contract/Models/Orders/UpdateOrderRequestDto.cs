namespace Peach.TriathlonShop.Api.Contract.Models.Orders;

public class UpdateOrderRequestDto
{
    public List<AddOrderRowDto> AddOrderRows { get; set; } = [];
    public string? CustomerId { get; set; }
}
