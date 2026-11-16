namespace FlightAware.Api.Client.Models;

public record FlightSchedulesRequestDto(DateTimeOffset DateStart, DateTimeOffset DateEnd, string? OriginAirportCode = null, string? DestinationAirportCode = null, string? Airline = null, int? FlightNumber = null, bool? IncludeCodeshares = null, bool? IncludeRegional = null, int? MaxPages = null, string? Cursor = null);
public record AirportFlightsRequestDto(string Id, string? MaxPages = null, string? Type = null, DateTimeOffset? StartDate = null, DateTimeOffset? EndDate = null, string? Cursor = null);
public record AirportNearbyRequestDto(double Latitude, double Longitude, int? Radius = null, string? RadiusUnit = null, int? MaxPages = null, string? Cursor = null);
