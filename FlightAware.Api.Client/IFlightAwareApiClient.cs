using FlightAware.Api.Client.Models;

namespace FlightAware.Api.Client;

public interface IFlightAwareApiClient
{
    Task<FlightSchedulesResponseDto> GetFlightSchedulesDeparturesAsync(FlightSchedulesRequestDto request, CancellationToken cancellationToken = default);
    Task<FlightAwareAirportFlightsResponseDto> GetAirportFlightsScheduledDeparturesAsync(AirportFlightsRequestDto request, CancellationToken cancellationToken = default);
    Task<FlightAwareAirportsResponseDto> GetNearbyAirportsAsync(AirportNearbyRequestDto request, CancellationToken cancellationToken = default);
}
