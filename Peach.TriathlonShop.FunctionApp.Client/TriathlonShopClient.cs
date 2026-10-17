using Microsoft.Extensions.Configuration;
using Peach.TriathlonShop.FunctionApp.Client.Models;
using System.Text.Json;

namespace Peach.TriathlonShop.FunctionApp.Client;

public class TriathlonShopClient(HttpClient httpClient, IConfiguration configuration) : ITriathlonShopClient
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task<Response<SignalRConnectionInfo>> GetNegotiationAsync(string hubName)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"negotiation/{hubName}", UriKind.Relative)
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
            Content = JsonSerializer.Deserialize<SignalRConnectionInfo>(responseContent, jsonSerializerOptions)
        };
    }

    public async Task<Response<string>> SetOrderConfirm(long orderId)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"order/{orderId}/confirm", UriKind.Relative)
        };

        var response = await httpClient.SendAsync(request);

        var responseContent = await response.Content.ReadAsStringAsync();

        return new Response<string>
        {
            IsSuccess = response.IsSuccessStatusCode,
            StatusCode = (int)response.StatusCode,
            Content = responseContent,
            ErrorMessage = response.IsSuccessStatusCode ? null : responseContent,
        };
    }
}

public class SignalRConnectionInfo
{
    public required string Url { get; set; }
    public required string AccessToken { get; set; }
}