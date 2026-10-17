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

builder.ConfigureFunctionsWebApplication();

/// Register Azure App Configuration
builder.AddCustomClientSecretAzureAppConfiguration();

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
var signalRConnectionString = builder.Configuration["AzureSignalRConnectionString"]
    ?? throw new InvalidOperationException("AzureSignalRConnectionString is missing.");

builder.Services.AddSingleton<IServiceManager>(_ =>
    (IServiceManager)new ServiceManagerBuilder()
        .WithOptions(o => o.ConnectionString = signalRConnectionString)
        .BuildServiceManager());

builder.Services.AddSingleton<SignalRNotifier>();

builder.Build().Run();
