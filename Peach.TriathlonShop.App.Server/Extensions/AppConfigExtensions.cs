using Microsoft.Extensions.Configuration;
using Peach.TriathlonShop.App.Server.Extensions.Options;

namespace Peach.TriathlonShop.App.Server.Extensions;

public class AppConfigExtensions
{
    public static PeachOptions GetPeachOptions(IConfiguration configuration)
    {
        var options = new PeachOptions();
        configuration.GetSection("Peach").Bind(options);
        return options;
    }

    public static TriathlonShopOptions GetTriathlonShopOptions(IConfiguration configuration)
    {
        var options = new TriathlonShopOptions();
        configuration.GetSection("TriathlonShop").Bind(options);
        return options;
    }
}
