using Azure.Storage.Queues.Models;
using FlightAware.Api.Client;
using FlightAware.Api.Client.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;
using Peach.TriathlonShop.FunctionApp.Services;

namespace Peach.TriathlonShop.FunctionApp.Queues;

public class CreateTransportOrderRequest(ILogger<CreateTransportOrderRequest> logger, TriathlonShopDbContext context, SignalRNotifier signalRNotifier, IFlightAwareApiClient flightAwareApiClient)
{
    public const string QueueName = "create-transport-order-request";

    [Function(nameof(CreateTransportOrderRequest))]
    public async Task Run([QueueTrigger(QueueName)] QueueMessage message)
    {
        var orderId = long.Parse(message.MessageText);

        await signalRNotifier.SendOrderMessageAsync(orderId, $"Transport request: Starting requesting transport.");

        var order = await context.Orders
            .Include(o => o.DestinationLocation)
                .ThenInclude(dl => dl.Airport)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found.");

        if (order.DestinationLocation.Airport == null || order.DestinationLocation.Airport.IcaoCode == null)
            throw new InvalidOperationException($"Order with ID {orderId} has no destination airport or ICAO code.");

        /// Transport origin location
        var warehouse = await context.WarehouseShipToCountries
            .Include(wsc => wsc.Warehouse)
                .ThenInclude(w => w.Location)
                    .ThenInclude(l => l.Airport)
            .Where(wsc => wsc.CountryId == order.DestinationLocation.CountryId)
            .Select(wsc => wsc.Warehouse)
            .FirstOrDefaultAsync();

        if (warehouse == null)
        {
            throw new InvalidOperationException($"No warehouse found for country ID {order.DestinationLocation.CountryId}.");
        }

        /// Domestic transport
        if (order.DestinationLocation.CountryId == warehouse.Location.CountryId)
        {
            logger.LogInformation("Order with ID {OrderId} has the same country as the warehouse. No flight request needed.", orderId);

            context.Add(new TransportOrder
            {
                OrderId = orderId,
                Status = Database.Models.Enums.TransportStatus.Requested,
                Destination = order.DestinationAddress,
                ConsignmentNumber = string.Empty,
                Carrier = "Road",
                EstimatedDeliveryDate = null
            });

            await context.SaveChangesAsync();
            return;
        }

        if (warehouse.Location.Airport == null || warehouse.Location.Airport.IcaoCode == null)
        {
            throw new InvalidOperationException($"Warehouse with ID {warehouse.Id} has no associated airport or ICAO code.");
        }

        var transportOrder = new TransportOrder
        {
            OrderId = orderId,
            Status = Database.Models.Enums.TransportStatus.Requested,
            ConsignmentNumber = string.Empty,
        };

        var originAirportIcao = warehouse.Location.Airport.IcaoCode;
        var destinationCityId = order.DestinationLocation.CityId;
        var destinationCountryId = order.DestinationLocation.CountryId;
        var destinationAirportIcao = order.DestinationLocation.Airport.IcaoCode;

        if (destinationCityId == null)
        {
            throw new InvalidOperationException($"Order with ID {orderId} has no destination city.");
        }

        bool isDestination = false;
        var scheduleDeparture = DateTimeOffset.UtcNow.AddDays(2).Date.AddHours(8);
        var flightSearchStartDateTime = scheduleDeparture;
        var flightSearchEndDateTime = flightSearchStartDateTime.AddDays(2).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

        var existingScheduleByCity = await GetExistingScheduledDestinationCity(scheduleDeparture, originAirportIcao, (long)destinationCityId);

        if (existingScheduleByCity != null && existingScheduleByCity.DestinationLocation == null)
        {
            throw new InvalidOperationException($"Existing schedule found for city ID {destinationCityId} but no destination location is associated with it.");
        }

        if (existingScheduleByCity != null)
        {
            transportOrder.Destination = existingScheduleByCity.DestinationLocation!.City.Name;
            transportOrder.ConsignmentNumber = existingScheduleByCity.IdentIcao;
            transportOrder.Carrier = existingScheduleByCity.Operator;
            transportOrder.ShippedAt = existingScheduleByCity.ScheduledDeparture;
            transportOrder.EstimatedDeliveryDate = existingScheduleByCity.ScheduledArrival;
            transportOrder.Status = Database.Models.Enums.TransportStatus.Accepted;

            await context.SaveChangesAsync();
            return;
        }

        /// Check lastest store flight scheduled date for the origin airport
        var lastFlightSchedule = await context.AirportSchedules
            .Where(s => s.OriginAirport.IcaoCode == originAirportIcao)
            .OrderByDescending(s => s.ScheduledDeparture)
            .FirstOrDefaultAsync();

        var flightSearchEndDateTimeLimit = DateTimeOffset.UtcNow.Date.AddDays(2).AddHours(23).AddMinutes(59).AddSeconds(59);

        if (lastFlightSchedule != null && lastFlightSchedule.ScheduledDeparture >= flightSearchEndDateTimeLimit)
        {
            transportOrder = await GetExistingScheduledFlightByCountry(scheduleDeparture, originAirportIcao, destinationCountryId, transportOrder);

            context.TransportOrders.Add(transportOrder);
            await context.SaveChangesAsync();

            return;
        }

        if (lastFlightSchedule != null && lastFlightSchedule.ScheduledDeparture > scheduleDeparture)
        {
            flightSearchStartDateTime = lastFlightSchedule.ScheduledDeparture.UtcDateTime;
        }

        int maxPages = 10;
        int currentPage = 1;
        int maxSet = 1;
        string? cursor = null;
        bool destinationFound = false;

        while (!destinationFound && currentPage <= maxPages)
        {
            FlightSchedulesResponseDto flights;

            try
            {
                flights = await flightAwareApiClient.GetFlightSchedulesDeparturesAsync(new FlightSchedulesRequestDto(
                    DateStart: flightSearchStartDateTime,
                    DateEnd: flightSearchEndDateTime,
                    OriginAirportCode: originAirportIcao,
                    MaxPages: maxSet,
                    Cursor: cursor
                ));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching airport flights for ICAO code {IcaoCode}.", originAirportIcao);
                // TODO: Save response
                return;
            }

            cursor = flights.Links?.Next?.Split("cursor=")?.LastOrDefault();

            try
            {
                FlightRequestEntity flightRequest = new()
                {
                    RequestParams = $"?id={originAirportIcao}",
                    ResponseBody = System.Text.Json.JsonSerializer.Serialize(flights),
                    Source = "FlightAware",
                    StatusCode = 200,
                };

                context.FlightRequests.Add(flightRequest);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving scheduled departures for airport flights for ICAO code {IcaoCode}.", originAirportIcao);
                return;
            }

            if (flights.Scheduled == null || flights.Scheduled.Count == 0)
            {
                logger.LogError("No scheduled departures found for airport flights for ICAO code {IcaoCode}.", originAirportIcao);
                return;
            }

            foreach (var flight in flights.Scheduled)
            {
                if (flight.IdentIcao == null || flight.IdentIata == null )
                {
                    logger.LogError("Missing required flight information for flight {Flight}.", flight);
                    return;
                }

                if (flight.ScheduledOut == null || flight.ScheduledIn == null)
                {
                    logger.LogError("Missing scheduled departure or arrival times for flight {Flight}.", flight);
                    return;
                }

                if (flight.Destination == null || flight.DestinationIcao == null)
                {
                    logger.LogError("Missing destination information for flight {Flight}.", flight);
                    return;
                }

                var destinationLocation = await context.Locations
                    .Include(l => l.Airport)
                    .FirstOrDefaultAsync(l => l.Airport != null && l.Airport.IcaoCode == flight.DestinationIcao);

                if (destinationLocation == null)
                {
                    logger.LogWarning("Destination location not found in database for flight {Flight}.", flight);
                }

                var airportSchedule = new AirportScheduleEntity
                {
                    IdentIcao = flight.IdentIcao,
                    IdentIata = flight.IdentIata,
                    Operator = flight.FaFlightId,
                    Status = string.Empty,
                    ScheduledDeparture = (DateTimeOffset)flight.ScheduledOut,
                    ScheduledArrival = (DateTimeOffset)flight.ScheduledIn,
                    ActualDeparture = null,
                    ActualArrival = null,
                    OriginAirportId = warehouse.Location.Airport.Id,
                    DestinationLocationId = destinationLocation?.Id,
                    DestinationAirportIata = flight.DestinationIata,
                    DestinationAirportIcao = flight.DestinationIcao,
                };

                context.AirportSchedules.Add(airportSchedule);
                await context.SaveChangesAsync();

                isDestination = flight.DestinationIcao == destinationAirportIcao;

                if (!destinationFound && isDestination)
                {
                    destinationFound = true;

                    transportOrder.Destination = flight.DestinationIcao;
                    transportOrder.ConsignmentNumber = flight.IdentIcao ?? string.Empty;
                    transportOrder.Carrier = flight.FaFlightId ?? string.Empty;
                    transportOrder.ShippedAt = flight.ScheduledOut;
                    transportOrder.EstimatedDeliveryDate = flight.ScheduledIn;
                    transportOrder.Status = Database.Models.Enums.TransportStatus.Accepted;
                    
                    await context.SaveChangesAsync();
                }
            }

            currentPage++;
        }

        transportOrder = await GetExistingScheduledFlightByCountry(scheduleDeparture, originAirportIcao, destinationCountryId, transportOrder);

        context.TransportOrders.Add(transportOrder);
        await context.SaveChangesAsync();

        await signalRNotifier.SendOrderMessageAsync(orderId, $"Transport request: Finished requesting transport.", isUpdate: true);
    }

    private async Task<TransportOrder> GetExistingScheduledFlightByCountry(DateTimeOffset scheduleDeparture, string originAirportIcao, long destinationCountryId, TransportOrder transportOrder)
    {
        var schedule = await context.AirportSchedules
                    .Include(s => s.DestinationLocation)
                        .ThenInclude(dl => dl == null ? null : dl.City)
                    .Where(s => s.ScheduledDeparture >= scheduleDeparture
                                && s.OriginAirport.IcaoCode == originAirportIcao
                                && s.DestinationLocation != null
                                && s.DestinationLocation.CountryId == destinationCountryId)
                    .OrderBy(s => s.ScheduledDeparture)
                    .FirstOrDefaultAsync();

        if (schedule != null && schedule.DestinationLocation != null)
        {
            transportOrder.Destination = schedule.DestinationLocation.City.Name;
            transportOrder.ConsignmentNumber = schedule.IdentIcao;
            transportOrder.Carrier = schedule.Operator;
            transportOrder.ShippedAt = schedule.ScheduledDeparture;
            transportOrder.EstimatedDeliveryDate = schedule.ScheduledArrival;
            transportOrder.Status = Database.Models.Enums.TransportStatus.Accepted;
        }

        return transportOrder;
    }

    private async Task<AirportScheduleEntity?> GetExistingScheduledDestinationCity(DateTimeOffset scheduleDeparture, string originAirportIcao, long destinationCityId)
    {
        var existingScheduleByCity = await context.AirportSchedules
            .Where(s => s.ScheduledDeparture >= scheduleDeparture
                        && s.OriginAirport.IcaoCode == originAirportIcao
                        && s.DestinationLocation != null
                        && s.DestinationLocation.CityId == destinationCityId)
            .OrderBy(s => s.ScheduledDeparture)
            .FirstOrDefaultAsync();

        return existingScheduleByCity;
    }
}