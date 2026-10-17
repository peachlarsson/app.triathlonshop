using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Peach.TriathlonShop.Database;

namespace Peach.TriathlonShop.FunctionApp.Extensions;

public static class ConfigurationExtensions
{
    public static FunctionsApplicationBuilder AddCustomAzureAppConfiguration(this FunctionsApplicationBuilder builder, TokenCredential credential, string[] labels)
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
                    refreshOptions.Register("*", label, refreshAll: true);
                }
            });

            options.ConfigureKeyVault(kv => kv.SetCredential(credential));
        });

        builder.Services.AddAzureAppConfiguration();
        return builder;
    }

    public static FunctionsApplicationBuilder AddCustomClientSecretAzureAppConfiguration(this FunctionsApplicationBuilder builder)
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            var appConfigEndpoint = builder.Configuration["AzureAppConfigEndpoint"]
                ?? throw new InvalidOperationException("Azure App Configuration endpoint is not configured.");

            TokenCredential credential = CreateClientSecretCredential(builder.Configuration);
            var labels = new[] { "common" };

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

    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = AppConfigExtensions.GetPeachOptions(configuration);

        try
        {
            services.AddDbContext<TriathlonShopDbContext>(opt => opt.UseSqlServer(options.DatabaseConnectionString));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return services;
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