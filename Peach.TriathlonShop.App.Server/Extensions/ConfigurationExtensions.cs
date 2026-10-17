using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace Peach.TriathlonShop.App.Server.Extensions;

public static class ConfigurationExtensions
{
    public static WebApplicationBuilder AddCustomAzureAppConfiguration(this WebApplicationBuilder builder, TokenCredential credential, string[] labels)
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            var appConfigEndpoint = builder.Configuration["AzureAppConfigEndpoint"]
                ?? throw new InvalidOperationException("Azure App Configuration endpoint is not configured.");

            options.Connect(new Uri(appConfigEndpoint), credential);

            foreach (var label in labels)
            {
                options.Select(KeyFilter.Any, label);
            }

            options.ConfigureRefresh(refreshOptions =>
            {
                foreach (var label in labels)
                {
                    refreshOptions.Register("*", label, true);
                }
            });

            options.ConfigureKeyVault(kv => kv.SetCredential(credential));
        });

        builder.Services.AddAzureAppConfiguration();
        return builder;
    }

    public static WebApplicationBuilder AddCustomClientSecretAzureAppConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            var appConfigEndpoint = builder.Configuration["AzureAppConfigEndpoint"]
                ?? throw new InvalidOperationException("Azure App Configuration endpoint is not configured.");

            TokenCredential credential = CreateClientSecretCredential(builder.Configuration);
            var labels = new[] { "common", "triathlonshop-web" };

            options.Connect(new Uri(appConfigEndpoint), credential);

            foreach (var label in labels)
            {
                options.Select(KeyFilter.Any, label);
            }

            options.ConfigureRefresh(refreshOptions =>
            {
                foreach (var label in labels)
                {
                    refreshOptions.Register("*", label, true);
                }
            });

            options.ConfigureKeyVault(kv => kv.SetCredential(CreateClientSecretCredential(builder.Configuration)));
        });

        builder.Services.AddAzureAppConfiguration();
        return builder;
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
