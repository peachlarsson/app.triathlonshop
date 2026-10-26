namespace Peach.TriathlonShop.Database.Models;

public class Location : BaseEntity
{
    public required string CountryCode { get; set; }
    public required string CountryName { get; set; }
    public required string City { get; set; }
}

