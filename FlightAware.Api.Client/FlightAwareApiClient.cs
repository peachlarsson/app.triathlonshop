using FlightAware.Api.Client.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace FlightAware.Api.Client;

public class FlightAwareApiClient(HttpClient httpClient) : IFlightAwareApiClient
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task<FlightSchedulesResponseDto> GetFlightSchedulesDeparturesAsync(FlightSchedulesRequestDto request, CancellationToken cancellationToken = default)
    {
        var requestUri = $"aeroapi/schedules/{request.DateStart:yyyy-MM-ddTHH:mm:ssZ}/{request.DateEnd:yyyy-MM-ddTHH:mm:ssZ}";

        var queryParameters = new Dictionary<string, string?>();

        if (!string.IsNullOrEmpty(request.OriginAirportCode))
        {
            queryParameters["origin"] = request.OriginAirportCode;
        }

        if (!string.IsNullOrEmpty(request.DestinationAirportCode))
        {
            queryParameters["destination"] = request.DestinationAirportCode;
        }

        if (!string.IsNullOrEmpty(request.Airline))
        {
            queryParameters["airline"] = request.Airline;
        }

        if (request.FlightNumber.HasValue)
        {
            queryParameters["flight_number"] = request.FlightNumber.Value.ToString();
        }

        if (request.IncludeCodeshares.HasValue)
        {
            queryParameters["include_codeshares"] = request.IncludeCodeshares.Value.ToString().ToLowerInvariant();
        }

        if (request.IncludeRegional.HasValue)
        {
            queryParameters["include_regional"] = request.IncludeRegional.Value.ToString().ToLowerInvariant();
        }

        if (request.MaxPages.HasValue)
        {
            queryParameters["max_pages"] = request.MaxPages.Value.ToString();
        }

        if (!string.IsNullOrEmpty(request.Cursor))
        {
            queryParameters["cursor"] = request.Cursor;
        }

        if (queryParameters.Count > 0)
        {
            requestUri = QueryHelpers.AddQueryString(requestUri, queryParameters);
        }

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(requestUri, UriKind.Relative)
        };

        var response = await httpClient.SendAsync(requestMessage, cancellationToken);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        var result = JsonSerializer.Deserialize<FlightSchedulesResponseDto>(responseContent, jsonSerializerOptions)
            ?? throw new InvalidOperationException("Failed to deserialize the response.");

        return result;
    }

    public async Task<FlightAwareAirportFlightsResponseDto> GetAirportFlightsScheduledDeparturesAsync(AirportFlightsRequestDto request, CancellationToken cancellationToken = default)
    {
        var requestUri = $"aeroapi/airports/{request.Id}/flights/scheduled_departures";

        if (!string.IsNullOrEmpty(request.Cursor))
        {
            requestUri += $"?cursor={request.Cursor}";
        }

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(requestUri, UriKind.Relative)
        };

        var response = await httpClient.SendAsync(requestMessage, cancellationToken);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        var result = JsonSerializer.Deserialize<FlightAwareAirportFlightsResponseDto>(responseContent, jsonSerializerOptions)
            ?? throw new InvalidOperationException("Failed to deserialize the response.");

        return result;
    }

    public Task<FlightAwareAirportsResponseDto> GetNearbyAirportsAsync(AirportNearbyRequestDto request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
