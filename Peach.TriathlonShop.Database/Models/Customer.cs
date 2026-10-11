namespace Peach.TriathlonShop.Database.Models;

public class Customer : BaseEntity
{
    public bool IsCompany { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.Column("CustomerName")]
    public string? Name { get; set; }
    public string? Email { get; set; }
}
