using System.ComponentModel;

namespace Peach.TriathlonShop.Api.Contract.Models.Stock;

public class MonthlyStockSales
{
    public int Month { get; set; }

    [Description("Total number of articles sold in the month.")]
    public int Sales { get; set; }
    public int Purchased { get; set; }
}
