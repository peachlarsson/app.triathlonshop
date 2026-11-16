using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlightAware.Api.Client.Models;

public class FlightSchedulesResponseDto : FlightAwareBaseDto
{
    [JsonPropertyName("links")]
    public FlightAwareLinksDto? Links { get; init; }


    [JsonPropertyName("num_pages")]
    public int? NumPages { get; init; }


    [JsonPropertyName("scheduled")]
    public IReadOnlyList<FlightScheduleDto>? Scheduled { get; init; }
}

public class FlightScheduleDto
{
    [JsonPropertyName("ident")]
    public string? Ident { get; init; }


    [JsonPropertyName("ident_icao")]
    public string? IdentIcao { get; init; }


    [JsonPropertyName("ident_iata")]
    public string? IdentIata { get; init; }


    [JsonPropertyName("actual_ident")]
    public string? ActualIdent { get; init; }


    [JsonPropertyName("actual_ident_icao")]
    public string? ActualIdentIcao { get; init; }


    [JsonPropertyName("actual_ident_iata")]
    public string? ActualIdentIata { get; init; }


    [JsonPropertyName("aircraft_type")]
    public string? AircraftType { get; init; }


    [JsonPropertyName("scheduled_in")]
    public DateTimeOffset? ScheduledIn { get; init; }


    [JsonPropertyName("scheduled_out")]
    public DateTimeOffset? ScheduledOut { get; init; }


    [JsonPropertyName("origin")]
    public string? Origin { get; init; }


    [JsonPropertyName("origin_icao")]
    public string? OriginIcao { get; init; }


    [JsonPropertyName("origin_iata")]
    public string? OriginIata { get; init; }


    [JsonPropertyName("origin_lid")]
    public string? OriginLid { get; init; }


    [JsonPropertyName("destination")]
    public string? Destination { get; init; }


    [JsonPropertyName("destination_icao")]
    public string? DestinationIcao { get; init; }


    [JsonPropertyName("destination_iata")]
    public string? DestinationIata { get; init; }


    [JsonPropertyName("destination_lid")]
    public string? DestinationLid { get; init; }


    [JsonPropertyName("fa_flight_id")]
    public string? FaFlightId { get; init; }


    [JsonPropertyName("meal_service")]
    public string? MealService { get; init; }


    [JsonPropertyName("seats_cabin_business")]
    public int? SeatsCabinBusiness { get; init; }


    [JsonPropertyName("seats_cabin_coach")]
    public int? SeatsCabinCoach { get; init; }


    [JsonPropertyName("seats_cabin_first")]
    public int? SeatsCabinFirst { get; init; }
}
