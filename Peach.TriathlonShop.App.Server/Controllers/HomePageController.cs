using Microsoft.AspNetCore.Mvc;
using Peach.TriathlonShop.FunctionApp.Client;

namespace Peach.TriathlonShop.App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomePageController(ITriathlonShopClient triathlonShopClient) : ControllerBase
    {
        [HttpGet("negotiation/{hubName}")]
        public async Task<SignalRConnectionInfo> GetNegotiationAsync(string hubName)
        {
            var response = await triathlonShopClient.GetNegotiationAsync(hubName: hubName);

            if (!response.IsSuccess || response.Content is null)
            {
                throw new Exception($"Failed to get SignalR negotiation info. StatusCode: {response.StatusCode}, ErrorMessage: {response.ErrorMessage}");
            }

            return new SignalRConnectionInfo
            {
                Url = response.Content.Url,
                AccessToken = response.Content.AccessToken,
            };
        }
    }
}
