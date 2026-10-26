using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.Database.Models;

public class Route : BaseEntity
{
    public long OrderId { get; set; }
    public RouteStatus RouteStatus { get; set; }
    public long StartLocationId { get; set; }
    public long EndLocationId { get; set; }

    public Location? StartLocation { get; set; } = default!;
    public Location? EndLocation { get; set; } = default!;
}
