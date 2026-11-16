namespace Peach.TriathlonShop.FunctionApp.Extensions.Options;

public class FlightAwareOptions
{
    public const string SectionName = "FlightAware";

    public string? BaseUrl { get; init; }
    public string? ApiKeyName { get; init; }
    public string? ApiKeyValue { get; init; }
    public string? EndpointSchedules { get; init; }
}
