using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Peach.TriathlonShop.FunctionApp.Extensions;

namespace Peach.TriathlonShop.FunctionApp.Developer;

public class DeveloperSendSignalRMessage(ILogger<DeveloperSendSignalRMessage> logger)
{
    [Function(nameof(DeveloperSendSignalRMessage))]
    public async Task<TestHttpOutput> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "developer/signalr/{hubName}/{eventName}")] HttpRequest req, string hubName, string eventName)
    {
        if (!SignalRHubName.All.Contains(hubName, StringComparer.OrdinalIgnoreCase))
        {
            return new TestHttpOutput
            {
                Response = new BadRequestObjectResult($"Invalid hub name '{hubName}'.")
            };
        }

        using var reader = new StreamReader(req.Body);
        var requestBody = await reader.ReadToEndAsync();

        Console.WriteLine($"Initializing SignalR message for hub '{hubName}' and event '{eventName}' with message: {requestBody}.");

        return new TestHttpOutput
        {
            Response = new OkResult(),
            SignalRMessage = new SignalRMessageAction(eventName)
            {
                Arguments =
                [
                    new
                    {
                        Message = requestBody,
                        Timestamp = DateTimeOffset.UtcNow,
                    }
                ]
            }
        };
    }
}

public class TestHttpOutput
{
    [HttpResult]
    public required IActionResult Response { get; set; }

    [SignalROutput(HubName = "{hubName}")]
    public SignalRMessageAction? SignalRMessage { get; set; }
}