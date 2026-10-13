using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Database;

var builder = WebApplication.CreateBuilder(args);

/// Database
var dbConnString = builder.Configuration.GetSection("DatabaseConnectionString").Value;

if (string.IsNullOrEmpty(dbConnString))
    throw new InvalidOperationException("Database connection string is not configured.");

builder.Services.AddDbContext<TriathlonShopDbContext>(opt => opt.UseSqlServer(dbConnString));

/// Service Bus
var serviceBusConnString = builder.Configuration.GetSection("ServiceBusConnectionString").Value;

if (string.IsNullOrEmpty(serviceBusConnString))
    throw new InvalidOperationException("Service Bus connection string is not configured.");

builder.Services.AddSingleton(_ => new ServiceBusClient(serviceBusConnString));

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();