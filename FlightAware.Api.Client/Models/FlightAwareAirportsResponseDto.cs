using System.Text.Json.Serialization;

namespace FlightAware.Api.Client.Models;

public class FlightAwareAirportsResponseDto : FlightAwareBaseDto
{
    [JsonPropertyName("links")]
    public FlightAwareLinksDto? Links { get; init; }

    [JsonPropertyName("num_pages")]
    public int? NumPages { get; init; }

    [JsonPropertyName("airports")]
    public IReadOnlyList<FlightAwareAirportDto>? Airports { get; init; }
}

public class FlightAwareAirportDto : FlightAwareBaseDto
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

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; init; }
}