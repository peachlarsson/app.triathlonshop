using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Peach.TriathlonShop.FunctionApp.Extensions;

namespace Peach.TriathlonShop.FunctionApp.Http;

public class NegotiateHttp()
{
    [Function(nameof(NegotiateHttp))]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "negotiation/{hubName}")] HttpRequest req,
        string hubName,
        [SignalRConnectionInfoInput(HubName = "{hubName}")] SignalRConnectionInfo testHubConnectionInfo)
    {
        if (!SignalRHubName.All.Contains(hubName, StringComparer.OrdinalIgnoreCase))
        {
            return new BadRequestObjectResult("Invalid hub name.");
        }

        return new OkObjectResult(testHubConnectionInfo);
    }
}