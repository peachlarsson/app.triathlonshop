using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.Database.Models;

public class Order : BaseEntity
{
    public required OrderStatus Status { get; set; }
    public required long CustomerId { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.Column("OrderNumber")]
    public required string Number { get; set; }

    public required decimal TotalPrice { get; set; }
    public List<OrderRow> OrderRows { get; set; } = [];

    public Customer? Customer { get; set; } = null!;
}
