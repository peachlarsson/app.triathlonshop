using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peach.TriathlonShop.FunctionApp.Client.AppConfig;

namespace Peach.TriathlonShop.FunctionApp.Client.ServiceCollection;

public static class ServiceCollection
{
    public static IServiceCollection AddTriathlonShopClient(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            var options = AppConfigExtensions.GetTriathlonShopOptions(configuration);

            if (options == null)
                throw new InvalidOperationException("Missing configurations.");

            if (options.FunctionApp == null)
                throw new InvalidOperationException("Missing configuration section: TriathlonShop/FunctionApp.");

            if (string.IsNullOrWhiteSpace(options.FunctionApp.BaseUrl)
                || string.IsNullOrWhiteSpace(options.FunctionApp.KeyName)
                || string.IsNullOrWhiteSpace(options.FunctionApp.KeyValue))
                throw new InvalidOperationException("Missing configuration values: TriathlonShop/FunctionApp.");

            services.AddHttpClient<ITriathlonShopClient, TriathlonShopClient>(client =>
            {
                client.BaseAddress = new Uri(options.FunctionApp.BaseUrl);
                client.DefaultRequestHeaders.Add(options.FunctionApp.KeyName, options.FunctionApp.KeyValue);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return services;
    }
}
