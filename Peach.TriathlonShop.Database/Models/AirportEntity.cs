namespace Peach.TriathlonShop.Database.Models;

public class AirportEntity : BaseEntity
{
    public const string TableName = "Airport";

    public string? IataCode { get; set; }
    public string? IcaoCode { get; set; }
    public string? Name { get; set; }
    public string? Timezone { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? IsoCountry { get; set; }
    public string? Municipality { get; set; }
}

