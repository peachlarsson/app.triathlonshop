namespace Peach.TriathlonShop.Database.Models;

public class FlightRequestEntity : BaseEntity
{
    public const string TableName = "FlightRequest";

    public required string RequestParams { get; set; }
    public string? ResponseBody { get; set; }
    public string? Source { get; set; }
    public int StatusCode { get; set; }
}
