using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peach.TriathlonShop.Database;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

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

builder.Build().Run();
