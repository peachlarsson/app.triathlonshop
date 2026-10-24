using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.App.shadcn.Server.Extensions;
using Peach.TriathlonShop.Database;

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
}
