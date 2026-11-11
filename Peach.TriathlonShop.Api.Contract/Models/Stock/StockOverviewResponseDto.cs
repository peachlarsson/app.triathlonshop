namespace Peach.TriathlonShop.Api.Contract.Models.Stock;

public class StockOverviewResponseDto
{
    public List<WarehouseOverviewDto> Warehouses { get; set; } = [];
    public List<MonthlyStockSales> MonthlyStockSales { get; set; } = [];
}
