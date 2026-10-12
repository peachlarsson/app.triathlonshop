namespace Peach.TriathlonShop.App.Server.Controllers.Models;

public class OrderDto
{
    public required long Id { get; set; }
    public required string Number { get; set; }
    public List<OrderRowDto> Items { get; set; } = [];
    public required decimal TotalPrice { get; set; }
    public int OrderRowsCount => Items.Count;
    public required long CustomerId { get; set; }
    public required string CustomerName { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
