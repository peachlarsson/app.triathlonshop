using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.App.shadcn.Server.Extensions;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.FunctionApp.Client;

namespace Peach.TriathlonShop.App.shadcn.Server.ServiceCollections;

public static class ServiceCollection
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = AppConfigExtensions.GetPeachOptions(configuration);

        try
        {
            services.AddDbContext<TriathlonShopDbContext>(opt => opt.UseSqlServer(options.DatabaseConnectionString));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return services;
    }

    public static IServiceCollection AddTriathlonShopClient(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            var options = AppConfigExtensions.GetTriathlonShopOptions(configuration);

            if (options == null)
                throw new InvalidOperationException("Missing configurations.");

            if (options.API == null)
                throw new InvalidOperationException("Missing configuration section: TriathlonShop/FunctionApp.");

            if (string.IsNullOrWhiteSpace(options.API.BaseUrl)
                || string.IsNullOrWhiteSpace(options.API.KeyName)
                || string.IsNullOrWhiteSpace(options.API.MasterKeyValue))
                throw new InvalidOperationException("Missing configuration values: TriathlonShop/FunctionApp.");

            services.AddHttpClient<ITriathlonShopClient, TriathlonShopClient>(client =>
            {
                client.BaseAddress = new Uri($"{options.API.BaseUrl}/api/");
                client.DefaultRequestHeaders.Add(options.API.KeyName, options.API.MasterKeyValue);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return services;
    }
}
