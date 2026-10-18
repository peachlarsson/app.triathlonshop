namespace Peach.TriathlonShop.FunctionApp.Extensions.Options;

public class PeachOptions
{
    public const string SectionName = "Peach";
    public string? DatabaseConnectionString { get; set; }
    public string? DatabaseConnectionStringEntra { get; set; }
    public string? ServiceBusFullyQualifiedNamespace { get; set; }
    public string? SignalRConnectionString { get; set; }
    public string? SignalRConnectionStringServiceUri { get; set; }
    public string? SignalRConnectionStringCredential { get; set; }
}
