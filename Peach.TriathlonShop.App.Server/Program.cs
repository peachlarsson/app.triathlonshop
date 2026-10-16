using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.App.Server.Extensions;
using Peach.TriathlonShop.App.Server.ServiceCollections;
using Peach.TriathlonShop.Database;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

TokenCredential credential;

#if DEBUG
    credential = builder.Configuration.CreateClientSecretCredential();
#else
    credential = new Azure.Identity.DefaultAzureCredential();
#endif

/// Register Azure App Configuration
builder.AddCustomAzureAppConfiguration(credential, ["triathlonshop-web"]);

/// Database
var dbConnString = builder.Configuration.GetSection("DatabaseConnectionString").Value;

if (string.IsNullOrEmpty(dbConnString))
    throw new InvalidOperationException("Database connection string is not configured.");

builder.Services.AddDbContext<TriathlonShopDbContext>(opt => opt.UseSqlServer(dbConnString));

/// TriathlonShop Function App Client
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