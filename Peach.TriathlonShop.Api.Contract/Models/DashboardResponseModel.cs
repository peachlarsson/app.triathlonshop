namespace Peach.TriathlonShop.Api.Contract.Models;


public class DashboardResponseModel
{
    public decimal TotalRevenue { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalOrders { get; set; }
    public int ActiveOrders { get; set; }
    public List<MonthlyRevenueModel> MonthlyRevenue { get; set; } = [];
    public List<RecentOrderModel> RecentOrdersLast30Days { get; set; } = [];
}

public class MonthlyRevenueModel
{
    public int Month { get; set; }
    public decimal Revenue { get; set; }
}

public class RecentOrderModel
{
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}