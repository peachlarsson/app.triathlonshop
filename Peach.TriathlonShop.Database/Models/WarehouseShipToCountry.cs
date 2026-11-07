namespace Peach.TriathlonShop.Database.Models;

public class WarehouseShipToCountry : BaseEntity
{
    public required long WarehouseId { get; set; }
    public required long CountryId { get; set; }

    public Warehouse Warehouse { get; set; } = default!;
    public CountryEntity Country { get; set; } = default!;
}
