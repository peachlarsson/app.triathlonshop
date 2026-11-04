using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.Database.Models;

public class Invoice : BaseEntity
{
    public required long OrderId { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("InvoiceStatus")]
    public required InvoiceStatus Status { get; set; }

    public Order? Order { get; set; } = null!;
}
