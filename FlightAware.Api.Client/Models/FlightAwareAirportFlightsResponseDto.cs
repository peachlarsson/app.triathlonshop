using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlightAware.Api.Client.Models;

public class FlightAwareBaseDto
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}

public class FlightAwareAirportFlightsResponseDto : FlightAwareBaseDto
{
    [JsonPropertyName("links")]
    public FlightAwareLinksDto? Links { get; init; }

    [JsonPropertyName("num_pages")]
    public int? NumPages { get; init; }

    [JsonPropertyName("arrivals")]
    public IReadOnlyList<FlightAwareFlightDto>? Arrivals { get; init; }

    [JsonPropertyName("departures")]
    public IReadOnlyList<FlightAwareFlightDto>? Departures { get; init; }

    [JsonPropertyName("scheduled_arrivals")]
    public IReadOnlyList<FlightAwareFlightDto>? ScheduledArrivals { get; init; }

    [JsonPropertyName("scheduled_departures")]
    public IReadOnlyList<FlightAwareFlightDto>? ScheduledDepartures { get; init; }

    [JsonPropertyName("flights")]
    public IReadOnlyList<FlightAwareFlightDto>? Flights { get; init; }
}

public class FlightAwareLinksDto
{
    [JsonPropertyName("next")]
    public string? Next { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}

public class FlightAwareFlightDto : FlightAwareBaseDto
{
      //"scheduled_off": "2026-11-13T15:20:00Z",
      //"actual_off": null,
      //"scheduled_on": "2026-11-13T16:00:00Z",
      //"actual_on": null,

    [JsonPropertyName("ident")]
    public string? Ident { get; init; }

    [JsonPropertyName("ident_icao")]
    public string? IdentIcao { get; init; }

    [JsonPropertyName("ident_iata")]
    public string? IdentIata { get; init; }

    [JsonPropertyName("fa_flight_id")]
    public string? FaFlightId { get; init; }

    [JsonPropertyName("operator")]
    public string? Operator { get; init; }

    [JsonPropertyName("aircraft_type")]
    public string? AircraftType { get; init; }

    [JsonPropertyName("scheduled_off")]
    public DateTimeOffset? ScheduledOff { get; init; }

    [JsonPropertyName("actual_off")]
    public DateTimeOffset? ActualOff { get; init; }

    [JsonPropertyName("scheduled_on")]
    public DateTimeOffset? ScheduledOn { get; init; }
    
    [JsonPropertyName("actual_on")]
    public DateTimeOffset? ActualOn { get; init; }

    [JsonPropertyName("origin")]
    public FlightAwareLocationDto? Origin { get; init; }

    [JsonPropertyName("destination")]
    public FlightAwareLocationDto? Destination { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }
}

public class FlightAwareLocationDto : FlightAwareBaseDto
{
    [JsonPropertyName("code")]
    public string? Code { get; init; }

    [JsonPropertyName("code_icao")]
    public string? CodeIcao { get; init; }

    [JsonPropertyName("code_iata")]
    public string? CodeIata { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("city")]
    public string? City { get; init; }
}