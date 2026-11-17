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

        var originAirportCode = warehouse.Location.Airport.IcaoCode;

        bool isDestination = false;
        var dateStart = DateTimeOffset.UtcNow.AddDays(2).Date.AddHours(8);

        /// Check if destination is already in the database
        var existingSchedule = await context.AirportSchedules
            .Where(s => s.ScheduledDeparture >= dateStart
                        && s.OriginAirport.IcaoCode == originAirportCode 
                        && s.DestinationAirport != null 
                        && s.DestinationAirport.IcaoCode == order.DestinationLocation.Airport.IcaoCode)
            .OrderBy(s => s.ScheduledDeparture)
            .FirstOrDefaultAsync();

        if (existingSchedule != null)
        {
            context.Add(new TransportOrder
            {
                OrderId = orderId,
                Status = Database.Models.Enums.TransportStatus.Accepted,
                Destination = existingSchedule.DestinationAirport?.IcaoCode,
                ConsignmentNumber = existingSchedule.IdentIcao,
                Carrier = existingSchedule.Operator,
                ShippedAt = existingSchedule.ScheduledDeparture,
                EstimatedDeliveryDate = existingSchedule.ScheduledArrival
            });

            await context.SaveChangesAsync();

            return;
        }

        /// Check lastest store flight scheduled date for the origin airport
        var lastFlightSchedule = await context.AirportSchedules
            .Where(s => s.OriginAirport.IcaoCode == originAirportCode)
            .OrderByDescending(s => s.ScheduledDeparture)
            .FirstOrDefaultAsync();

        if (lastFlightSchedule != null && lastFlightSchedule.ScheduledDeparture > dateStart)
        {
            dateStart = lastFlightSchedule.ScheduledDeparture.UtcDateTime;
        }

        if (dateStart.Date >= DateTimeOffset.UtcNow.Date.AddDays(14))
        {
            logger.LogError("Next available flight schedule for airport {IcaoCode} is beyond 14 days from now. No flight request will be made.", originAirportCode);
            return;
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
                    DateStart: dateStart,
                    DateEnd: dateStart.AddDays(3).Date,
                    OriginAirportCode: originAirportCode,
                    MaxPages: maxSet,
                    Cursor: cursor
                ));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching airport flights for ICAO code {IcaoCode}.", originAirportCode);
                // TODO: Save response
                return;
            }

            cursor = flights.Links?.Next?.Split("cursor=")?.LastOrDefault();

            try
            {
                FlightRequestEntity flightRequest = new()
                {
                    RequestParams = $"?id={originAirportCode}",
                    ResponseBody = System.Text.Json.JsonSerializer.Serialize(flights),
                    Source = "FlightAware",
                    StatusCode = 200,
                };

                context.FlightRequests.Add(flightRequest);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving scheduled departures for airport flights for ICAO code {IcaoCode}.", originAirportCode);
                return;
            }

            if (flights.Scheduled == null || flights.Scheduled.Count == 0)
            {
                logger.LogError("No scheduled departures found for airport flights for ICAO code {IcaoCode}.", originAirportCode);
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

                var commonDestination = await context.Airports
                    .FirstOrDefaultAsync(a => a.IcaoCode == flight.DestinationIcao);

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
                    DestinationAirportId = commonDestination?.Id,
                };

                context.AirportSchedules.Add(airportSchedule);

                isDestination = order.DestinationLocation.Airport.IcaoCode == flight.DestinationIcao;

                if (!destinationFound && isDestination)
                {
                    destinationFound = true;

                    transportOrder.Destination = flight.DestinationIcao;
                    transportOrder.ConsignmentNumber = flight.IdentIcao ?? string.Empty;
                    transportOrder.Carrier = flight.FaFlightId ?? string.Empty;
                    transportOrder.ShippedAt = flight.ScheduledOut;
                    transportOrder.EstimatedDeliveryDate = flight.ScheduledIn;
                    transportOrder.Status = Database.Models.Enums.TransportStatus.Accepted;
                }
            }

            currentPage++;
        }

        context.TransportOrders.Add(transportOrder);
        await context.SaveChangesAsync();

        await signalRNotifier.SendOrderMessageAsync(orderId, $"Transport request: Finished requesting transport.", isUpdate: true);
    }
}