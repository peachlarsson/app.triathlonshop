namespace Peach.TriathlonShop.Database.Models;

public class City : BaseEntity
{
    public required string Name { get; set; }
    public required string CountryCode { get; set; }
    public long? Population { get; set; }
    public string? Timezone { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? Elevation { get; set; }
}