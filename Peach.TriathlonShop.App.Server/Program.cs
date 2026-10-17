using Azure.Core;
using Peach.TriathlonShop.App.Server.Extensions;
using Peach.TriathlonShop.App.Server.ServiceCollections;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

/// Register Azure App Configuration
builder.AddCustomClientSecretAzureAppConfiguration();

/// Database
builder.Services.AddDatabaseConfiguration(builder.Configuration);

/// TriathlonShop API Client
builder.Services.AddTriathlonShopClient(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAzureAppConfiguration();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();