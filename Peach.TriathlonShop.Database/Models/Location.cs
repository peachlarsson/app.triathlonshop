namespace Peach.TriathlonShop.Database.Models;

public class Location : BaseEntity
{
    public required long CountryId { get; set; }
    public long? CityId { get; set; }

    public CountryEntity Country { get; set; } = default!;
    public City City { get; set; } = default!;
}

