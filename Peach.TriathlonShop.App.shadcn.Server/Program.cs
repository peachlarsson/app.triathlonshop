using Azure.Core;
using Peach.TriathlonShop.App.shadcn.Server.Extensions;
using Peach.TriathlonShop.App.shadcn.Server.ServiceCollections;
using System.Text.Json.Serialization;
using ConfigurationExtensions = Peach.TriathlonShop.App.shadcn.Server.Extensions.ConfigurationExtensions;

var builder = WebApplication.CreateBuilder(args);

TokenCredential credential;

#if DEBUG
credential = ConfigurationExtensions.CreateClientSecretCredential(builder.Configuration);
#else
credential = new Azure.Identity.ManagedIdentityCredential(Azure.Identity.ManagedIdentityId.SystemAssigned);
#endif

/// Register Azure App Configuration
builder.AddCustomAzureAppConfiguration(credential, ["common", "triathlonshop", "triathlonshop-web"]);

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