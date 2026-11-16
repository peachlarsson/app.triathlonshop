using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace RunBatch;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddCustomAzureAppConfiguration(this IConfigurationBuilder configuration, TokenCredential credential, string[] labels)
    {
        var builtConfiguration = configuration.Build();
        var appConfigEndpoint = builtConfiguration["AzureAppConfigEndpoint"]
            ?? throw new InvalidOperationException("Azure App Configuration endpoint is not configured.");

        configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(new Uri(appConfigEndpoint), credential);

            foreach (var label in labels)
            {
                options.Select(KeyFilter.Any, label);
            }

            options.ConfigureRefresh(refreshOptions =>
            {
                foreach (var label in labels)
                {
                    refreshOptions.Register("*", label, refreshAll: true);
                }
            });

            options.ConfigureKeyVault(kv => kv.SetCredential(credential));
        });

        return configuration;
    }

    public static TokenCredential CreateClientSecretCredential(this IConfiguration configuration)
    {
        var tenantId = configuration["AzureAd:TenantId"]
            ?? throw new InvalidOperationException("Missing AzureAd:TenantId");

        var clientId = configuration["AzureAd:ClientId"]
            ?? throw new InvalidOperationException("Missing AzureAd:ClientId");

        var clientSecret = configuration["AzureAd:ClientSecret"]
            ?? throw new InvalidOperationException("Missing AzureAd:ClientSecret");

        return new ClientSecretCredential(tenantId, clientId, clientSecret);
    }
}
