namespace Peach.TriathlonShop.Database.Models;

public class Customer : BaseEntity
{
    public bool IsGuest { get; set; }
    public string? Email { get; set; }
}
