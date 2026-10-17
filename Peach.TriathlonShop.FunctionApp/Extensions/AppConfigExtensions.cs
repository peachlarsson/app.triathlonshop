using Microsoft.Extensions.Configuration;
using Peach.TriathlonShop.FunctionApp.Extensions.Options;

namespace Peach.TriathlonShop.FunctionApp.Extensions;

public class AppConfigExtensions
{
    public static PeachOptions GetPeachOptions(IConfiguration configuration)
    {
        var options = new PeachOptions();
        configuration.GetSection("Peach").Bind(options);
        return options;
    }
}
