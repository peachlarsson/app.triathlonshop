namespace Peach.TriathlonShop.Api.Contract.Models.Orders;

public class UpdateOrderRequestDto
{
    public List<AddOrderRowRequestDto> AddOrderRows { get; set; } = [];
    public List<UpdateOrderRowRequestDto> UpdateOrderRows { get; set; } = [];
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
}

public class AddOrderRowRequestDto
{
    public long ArticleId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateOrderRowRequestDto
{
    public int OrderRowId { get; set; }
    public string? ArticleName { get; set; }
    public int Quantity { get; set; }
}