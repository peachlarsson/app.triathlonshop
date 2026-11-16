using Azure.Core;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;
using RunBatch;
using System.Globalization;

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>(optional: true);

var baseConfiguration = builder.Build();

TokenCredential credential = baseConfiguration.CreateClientSecretCredential();

builder.AddCustomAzureAppConfiguration(credential: credential, labels: ["common", "triathlonshop", "triathlonshop-func"]);

var configuration = builder.Build();

var connectionString = configuration["Peach:DatabaseConnectionString"]
    ?? throw new InvalidOperationException("No database connection string found.");

var optionsBuilder = new DbContextOptionsBuilder<TriathlonShopDbContext>();
optionsBuilder.UseSqlServer(connectionString);

await using var context = new TriathlonShopDbContext(optionsBuilder.Options);

#region Select run mode

Console.WriteLine("Select run mode:");
Console.WriteLine($"1: {nameof(RunSaveCities)}");
Console.WriteLine($"2: {nameof(RunMapAirportInLocation)}");
Console.WriteLine($"3: {nameof(RunMapCityInLocation)}");
Console.WriteLine($"4: {nameof(RunImportAirports)}");

var runMode = Console.ReadLine();

switch (runMode)
{
    case "1":
        await RunSaveCities(context);
        break;
    case "2":
        await RunMapAirportInLocation(context);
        break;
    case "3":
        await RunMapCityInLocation(context);
        break;
    case "4":
        await RunImportAirports(context);
        break;
    default:
        Console.WriteLine("Invalid run mode selected.");
        break;
}

#endregion

static async Task RunSaveCities(TriathlonShopDbContext context)
{
    var filePath = @"C:\Users\peach\repos\app.triathlonshop\RunBatch\Data\cities500000.txt";

    using var reader = new StreamReader(filePath);

    var line = await reader.ReadLineAsync()
        ?? throw new InvalidOperationException("City data file is empty.");

    var fields = line.Split('\t');

    var cities = File.ReadLines(filePath)
        .Where(line => !line.StartsWith('#'))
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => line.Split('\t'))
        .Select(fields => new Peach.TriathlonShop.Database.Models.City
        {
            Name = fields[1],
            Latitude = decimal.Parse(fields[4], CultureInfo.InvariantCulture),
            Longitude = decimal.Parse(fields[5], CultureInfo.InvariantCulture),
            CountryCode = fields[8],
            Population = long.Parse(fields[14]),
            Elevation = int.TryParse(fields[15], out var elevation)
                ? elevation : null,
            Timezone = fields[17],
        });

    context.Cities.AddRange(cities);
    await context.SaveChangesAsync();
}

static async Task RunMapAirportInLocation(TriathlonShopDbContext context)
{
    int missingCountry = 0;
    int missingCityAdded = 0;
    int alreadyHasCompleteData = 0;
    int addCompleteLocation = 0;
    int totalProcessed = 0;

    using var beginTransaction = await context.Database.BeginTransactionAsync();

    var airports = await context.Airports
        .ToListAsync();

    int totalAirports = airports.Count;

    foreach (var airport in airports)
    {
        totalProcessed++;

        var country = await context.Countries
            .Where(c => c.Code == airport.IsoCountry)
            .FirstOrDefaultAsync();

        if (country == null)
        {
            missingCountry++;
            Console.WriteLine($"Country with code {airport.IsoCountry} not found for airport {airport.Name}.");
            continue;
        }

        var completeLocation = await context.Locations
            .Where(l => l.AirportId == airport.Id && l.City.Name == airport.Municipality && l.Country.Code == airport.IsoCountry)
            .FirstOrDefaultAsync();

        if (completeLocation != null)
        {
            alreadyHasCompleteData++;
            Console.WriteLine($"Has complete location. Airport: {airport.Name}, City: {completeLocation.City.Name}, Country: {completeLocation.Country.Code}");
            continue;
        }

        var city = await context.Cities
            .Where(c => c.Name == airport.Municipality && c.CountryCode == airport.IsoCountry)
            .FirstOrDefaultAsync();

        if (city == null)
        {
            missingCityAdded++;

            context.Locations.Add(new Location
            {
                CountryId = country.Id,
                CityId = null,
                AirportId = airport.Id,
            });

            continue;
        }

        context.Locations.Add(new Location
        {
            CountryId = country.Id,
            CityId = city.Id,
            AirportId = airport.Id,
        });
        
        addCompleteLocation++;
    }

    Console.WriteLine($"Total count: {totalAirports}.\n\nProcessed: {totalProcessed}.\n\nMissing Country: {missingCountry}.\n\nMissing City: {missingCityAdded}.\n\nAlready has Complete Data: {alreadyHasCompleteData}\n\nAdded Complete Location: {addCompleteLocation}");

    await context.SaveChangesAsync();
    await beginTransaction.CommitAsync();
}

static async Task RunMapCityInLocation(TriathlonShopDbContext context)
{
    int missingCountry = 0;
    int missingCityAdded = 0;
    int alreadyHasCompleteData = 0;
    int addCityLocation = 0;
    int totalProcessed = 0;

    using var beginTransaction = await context.Database.BeginTransactionAsync();

    var cities = await context.Cities
        .ToListAsync();

    int totalCities = cities.Count;

    foreach (var city in cities)
    {
        totalProcessed++;

        var country = await context.Countries
            .Where(c => c.Code == city.CountryCode)
            .FirstOrDefaultAsync();

        if (country == null)
        {
            missingCountry++;
            Console.WriteLine($"Country with code {city.CountryCode} not found for city {city.Name}.");
            continue;
        }

        var completeLocation = await context.Locations
            .Where(l => l.CityId == city.Id && l.CountryId == country.Id)
            .FirstOrDefaultAsync();

        if (completeLocation != null)
        {
            alreadyHasCompleteData++;
            Console.WriteLine($"Has complete location. City: {city.Name}, Country: {country.Code}");
            continue;
        }

        context.Locations.Add(new Location
        {
            CountryId = country.Id,
            CityId = city.Id,
            AirportId = null,
        });

        addCityLocation++;
    }

    Console.WriteLine($"Total count: {totalCities}.\n\nProcessed: {totalProcessed}.\n\nMissing Country: {missingCountry}.\n\nMissing City: {missingCityAdded}.\n\nAlready has Complete Data: {alreadyHasCompleteData}\n\nAdded City Location: {addCityLocation}");
    
    await context.SaveChangesAsync();
    await beginTransaction.CommitAsync();
}

static async Task RunImportAirports(TriathlonShopDbContext context)
{
    var filePath = @"C:\Users\peach\repos\app.triathlonshop\RunBatch\Data\airports.csv";

    using var reader = new StreamReader(filePath);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    var airports = csv.GetRecords<AirportCsvRow>()
        .Where(row => row.type == "large_airport")
        .Select(row =>
        {
            string? Get(string? value)
                => string.IsNullOrWhiteSpace(value) ? null : value;

            return new AirportEntity
            {
                IataCode = Get(row.iata_code),
                IcaoCode = Get(row.icao_code),
                Name = Get(row.name) ?? "",
                Latitude = decimal.TryParse(
                    Get(row.latitude_deg),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var lat) ? lat : null,
                Longitude = decimal.TryParse(
                    Get(row.longitude_deg),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var lon) ? lon : null,
                IsoCountry = Get(row.iso_country),
                Municipality = Get(row.municipality)
            };
        })
        .ToList();

    if (airports.Any(a => a.IcaoCode == "yes"))
    {
        Console.WriteLine("Error: Found airport with ICAO code 'yes'.");
        return;
    }

    context.Airports.AddRange(airports);
    await context.SaveChangesAsync();
}
public sealed class AirportCsvRow
{
    public string? type { get; set; }
    public string? iata_code { get; set; }
    public string? icao_code { get; set; }
    public string? name { get; set; }
    public string? latitude_deg { get; set; }
    public string? longitude_deg { get; set; }
    public string? iso_country { get; set; }
    public string? municipality { get; set; }
}
