namespace Peach.TriathlonShop.Database.Models;

public class Customer : BaseEntity
{
    public bool IsGuest { get; set; }
    public string? Email { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.Column("CustomerName")]
    public string? Name { get; set; }
}
