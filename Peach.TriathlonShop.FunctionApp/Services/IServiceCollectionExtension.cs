using FlightAware.Api.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Peach.TriathlonShop.FunctionApp.Extensions.Options;

namespace Peach.TriathlonShop.FunctionApp.Services;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddFlightAwareApiClient(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHttpClient<IFlightAwareApiClient, FlightAwareApiClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<FlightAwareOptions>>().Value;

            var baseUrl = options.BaseUrl;
            var apiKeyName = options.ApiKeyName;
            var apiKeyValue = options.ApiKeyValue;

            if (string.IsNullOrWhiteSpace(baseUrl) ||
                string.IsNullOrWhiteSpace(apiKeyName) ||
                string.IsNullOrWhiteSpace(apiKeyValue))
            {
                throw new InvalidOperationException("BaseUrl, API key name or API key value are null or empty.");
            }

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add(apiKeyName, apiKeyValue);
        });

        return services;
    }
}
