using Azure.Core;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.FunctionApp.Extensions;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

TokenCredential credential = new Azure.Identity.DefaultAzureCredential();

#if DEBUG
credential = builder.Configuration.CreateClientSecretCredential();
#else
    credential = new Azure.Identity.DefaultAzureCredential();
#endif

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

/// Database
var dbConnString = builder.Configuration.GetSection("DatabaseConnectionString").Value;

if (string.IsNullOrEmpty(dbConnString))
    throw new InvalidOperationException("Database connection string is not configured.");

builder.Services.AddDbContext<TriathlonShopDbContext>(opt => opt.UseSqlServer(dbConnString));

/// Service Bus
builder.Services.AddAzureClients(azure =>
{
    var serviceBusUri = builder.Configuration.GetSection("ServiceBus").GetSection("FullyQualifiedNamespace").Value;

    if (string.IsNullOrEmpty(serviceBusUri))
        throw new InvalidOperationException("Service Bus connection is not configured.");

    azure.AddServiceBusClientWithNamespace(serviceBusUri);
    azure.UseCredential(credential);
});


builder.Build().Run();
