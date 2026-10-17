using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peach.TriathlonShop.FunctionApp.Extensions;

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
builder.Services.AddAzureClients(azure =>
{
    var serviceBusUri = builder.Configuration.GetSection("ServiceBus").GetSection("FullyQualifiedNamespace").Value;

    if (string.IsNullOrEmpty(serviceBusUri))
        throw new InvalidOperationException("Service Bus connection is not configured.");

    azure.AddServiceBusClientWithNamespace(serviceBusUri);
    azure.UseCredential(builder.Configuration.CreateClientSecretCredential());
});


builder.Build().Run();
