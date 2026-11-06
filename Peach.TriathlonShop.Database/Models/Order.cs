using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.Database.Models;

public class Order : BaseEntity
{
    [System.ComponentModel.DataAnnotations.Schema.Column("OrderStatus")]
    public required OrderStatus Status { get; set; }
    public required long CustomerId { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.Column("OrderNumber")]
    public required string Number { get; set; }

    public required decimal TotalPrice { get; set; }
    public required DateTimeOffset PlacedAt { get; set; }
    public long? DestinationCityId { get; set; }
    public required long DestinationLocationId { get; set; }
    public string? DestinationAddress { get; set; }
    public List<OrderRow> OrderRows { get; set; } = [];
    public List<Notification> Notifications { get; set; } = [];
    public List<Invoice> Invoices { get; set; } = [];

    public Customer? Customer { get; set; } = null!;
    public City? DestinationCity { get; set; }
    public Location DestinationLocation { get; set; } = null!;
}
