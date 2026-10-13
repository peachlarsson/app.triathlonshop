using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.FunctionApp.Http.Models;
using Peach.TriathlonShop.FunctionApp.Queues;
using System.Text.Json;

namespace Peach.TriathlonShop.FunctionApp.Http
{
    public class TransportOrderAccepted(ILogger<TransportOrderAccepted> logger, TriathlonShopDbContext context)
    {
        [Function(nameof(TransportOrderAccepted))]
        public async Task<TransportOrderAcceptedOutput> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "transport-order/accepted/{consignmentNumber}")] HttpRequest req, string consignmentNumber)
        {
            using var body = req.Body;
            var requestBody = await new StreamReader(body).ReadToEndAsync();

            TransportOrderAcceptedEvent transportInfo;

            try
            {
                transportInfo = JsonSerializer.Deserialize<TransportOrderAcceptedEvent>(requestBody)!;
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to deserialize the request body into TransportOrderAcceptedEvent.");

                return new TransportOrderAcceptedOutput
                {
                    HttpResult = new BadRequestResult()
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while processing the request.");
                
                return new TransportOrderAcceptedOutput
                {
                    HttpResult = new StatusCodeResult(StatusCodes.Status500InternalServerError)
                };
            }

            return new TransportOrderAcceptedOutput
            {
                TransportOrderAcceptedQueueMessage = requestBody,
                HttpResult = new OkResult()
            };
        }
    }
}

namespace Peach.TriathlonShop.FunctionApp.Http.Models
{
    public class TransportOrderAcceptedEvent
    {
        public required string ConsignmentNumber { get; set; }
        public required string Destination { get; set; }
        public required string TrackingUrl { get; set; }
        public required DateTimeOffset EstimatedDeliveryDate { get; set; }
        public required DateTimeOffset Timestamp { get; set; }
    }

    public class TransportOrderAcceptedOutput
    {
        [QueueOutput(TransportOrderAcceptedQueue.QueueName)]
        public string? TransportOrderAcceptedQueueMessage { get; set; }

        [HttpResult]
        public required IActionResult HttpResult { get; set; }
    }
}
