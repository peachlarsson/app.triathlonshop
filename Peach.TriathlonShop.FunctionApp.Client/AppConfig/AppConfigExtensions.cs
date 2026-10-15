using Microsoft.Extensions.Configuration;

namespace Peach.TriathlonShop.FunctionApp.Client.AppConfig;

public class AppConfigExtensions
{
    public static TriathlonShopOptions GetTriathlonShopOptions(IConfiguration configuration)
    {
        var options = new TriathlonShopOptions();
        configuration.Bind(options);
        return options;
    }
}
