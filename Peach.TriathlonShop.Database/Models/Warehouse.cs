namespace Peach.TriathlonShop.Database.Models;

public class Warehouse : BaseEntity
{
    public required string WarehouseName { get; set; }
    public long LocationId { get; set; }

    public Location Location { get; set; } = default!;

    public ICollection<WarehouseShipToCountry> WarehouseShipToCountries { get; set; } = [];
}
