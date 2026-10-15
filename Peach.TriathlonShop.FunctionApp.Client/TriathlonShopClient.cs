using Microsoft.Extensions.Configuration;
using Peach.TriathlonShop.FunctionApp.Client.AppConfig;
using Peach.TriathlonShop.FunctionApp.Client.Models;
using System.Text.Json;

namespace Peach.TriathlonShop.FunctionApp.Client;

public class TriathlonShopClient(HttpClient httpClient, IConfiguration configuration) : ITriathlonShopClient
{
    public async Task<Response<SignalRConnectionInfo>> GetNegotiationAsync(string hubName)
    {
        var appConfig = AppConfigExtensions.GetTriathlonShopOptions(configuration);

        if (appConfig.FunctionApp == null)
            throw new InvalidOperationException("Missing configuration section: FunctionApp");

        if (string.IsNullOrWhiteSpace(appConfig.FunctionApp.EndpointGetNegotiation))
            throw new InvalidOperationException("Value is null or empty for configuration key: FunctionApp:EndpointGetNegotiation");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{appConfig.FunctionApp.EndpointGetNegotiation.Replace("{hubName}", hubName)}")
        };

        var response = await httpClient.SendAsync(request);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new Response<SignalRConnectionInfo>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = $"responseContent: {responseContent}"
            };
        }

        return new Response<SignalRConnectionInfo>
        {
            IsSuccess = true,
            StatusCode = (int)response.StatusCode,
            Content = JsonSerializer.Deserialize<SignalRConnectionInfo>(responseContent)
        };
    }
}

public class SignalRConnectionInfo
{
    public required string Url { get; set; }
    public required string AccessToken { get; set; }
}