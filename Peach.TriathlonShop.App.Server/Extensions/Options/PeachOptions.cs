namespace Peach.TriathlonShop.App.Server.Extensions.Options;

public class PeachOptions
{
    public const string SectionName = "Peach";
    public string? DatabaseConnectionString { get; set; }
    public string? DatabaseConnectionStringEntra { get; set; }
    public string? ServiceBusFullyQualifiedNamespace { get; set; }
}
