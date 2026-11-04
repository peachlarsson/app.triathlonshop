namespace Peach.TriathlonShop.Api.Contract.Developer.Models;

public class CreateRandomOrdersModel
{
    public int? OrdersCountIntervalStart { get; set; }
    public int? OrdersCountIntervalEnd { get; set; }
    public int? ArticlesCountIntervalStart { get; set; }
    public int? ArticlesCountIntervalEnd { get; set; }
    public int? QuantityIntervalStart { get; set; }
    public int? QuantityIntervalEnd { get; set; }
    public bool UseRandomDelay { get; set; }
    public int? DelayMsIntervalStart { get; set; }
    public int? DelayMsIntervalEnd { get; set; }
    public bool UseRandomOrderStatus { get; set; }
    public bool UseRandomPlacedDate { get; set; }
}