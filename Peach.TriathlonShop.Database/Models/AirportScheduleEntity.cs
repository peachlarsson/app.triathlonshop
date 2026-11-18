namespace Peach.TriathlonShop.Database.Models;

public class AirportScheduleEntity : BaseEntity
{
    public const string TableName = "AirportSchedule";

    public required string IdentIcao { get; set; }
    public required string IdentIata { get; set; }
    public string? Operator { get; set; }
    public string? Status { get; set; }

    public long OriginAirportId { get; set; }
    public long? DestinationLocationId { get; set; }
    public string? DestinationAirportIcao { get; set; }
    public string? DestinationAirportIata { get; set; }

    public required DateTimeOffset ScheduledDeparture { get; set; }
    public required DateTimeOffset ScheduledArrival { get; set; }
    public DateTimeOffset? ActualDeparture { get; set; }
    public DateTimeOffset? ActualArrival { get; set; }

    public AirportEntity OriginAirport { get; set; } = null!;
    public Location? DestinationLocation { get; set; }
}
