using Azure.Core;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peach.TriathlonShop.FunctionApp.Extensions;
using Peach.TriathlonShop.FunctionApp.Services;

var builder = FunctionsApplication.CreateBuilder(args);

TokenCredential credential;

#if DEBUG
credential = ConfigurationExtensions.CreateClientSecretCredential(builder.Configuration);
#else
credential = new Azure.Identity.ManagedIdentityCredential(Azure.Identity.ManagedIdentityId.SystemAssigned);
#endif

builder.ConfigureFunctionsWebApplication();

/// Register Azure App Configuration
builder.AddCustomAzureAppConfiguration(credential: credential, labels: ["triathlonshop-func", "common"]);

/// OpenTelemetry
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

/// Database
builder.Services.AddDatabaseConfiguration(builder.Configuration);

/// Service Bus
builder.Services.AddServiceBusClientWithNamespace(builder.Configuration);

/// SignalR
builder.Services.AddSignalRConnection(builder.Configuration);

builder.Build().Run();
