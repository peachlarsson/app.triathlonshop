namespace Peach.TriathlonShop.Api.Contract.Models;

public class LocationDto
{
    public required string CountryCode { get; set; }
    public string? CountryName { get; set; }
    public required string CityName { get; set; }
}
