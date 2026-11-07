namespace Peach.TriathlonShop.Database.Models;

public class CountryEntity : BaseEntity
{
    public readonly static string TableName = "Country";
    public required string Code { get; set; }
    public string? Name { get; set; }
    public required string ContinentCode { get; set; }
    public required string ContinentName { get; set; }

    public ICollection<WarehouseShipToCountry> WarehouseShipToCountries { get; set; } = [];
}