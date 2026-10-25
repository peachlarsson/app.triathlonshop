using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Api.Contract.Models;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.FunctionApp.Http;

public class DashboardHttp(IDbContextFactory<TriathlonShopDbContext> contextFactory)
{
    [Function(nameof(DashboardHttp))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "dashboard")] HttpRequest req)
    {
        var totalRevenueTask = GetTotalRevenue();
        var totalCustomersTask = GetTotalCustomers();
        var totalOrdersTask = GetTotalOrders();
        var activeOrdersTask = GetActiveOrders();
        var monthlyRevenueTask = GetMonthlyRevenue();
        var recentOrdersTask = GetRecentOrdersLast30Days();

        await Task.WhenAll(totalRevenueTask, totalCustomersTask, totalOrdersTask, activeOrdersTask, monthlyRevenueTask, recentOrdersTask);

        DashboardResponseModel response = new()
        {
            TotalRevenue = totalRevenueTask.Result,
            TotalCustomers = totalCustomersTask.Result,
            TotalOrders = totalOrdersTask.Result,
            ActiveOrders = activeOrdersTask.Result,
            MonthlyRevenue = monthlyRevenueTask.Result,
            RecentOrdersLast30Days = recentOrdersTask.Result,
        };

        return new OkObjectResult(response);
    }

    private async Task<decimal> GetTotalRevenue()
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Orders.AsNoTracking()
            .Where(o => o.Status == OrderStatus.Shipped)
            .SumAsync(o => o.TotalPrice);
    }

    private async Task<int> GetTotalCustomers()
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Customers.AsNoTracking().CountAsync();
    }

    private async Task<int> GetTotalOrders()
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Orders.AsNoTracking().CountAsync();
    }

    private async Task<int> GetActiveOrders()
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Orders.AsNoTracking()
            .Where(o => o.Status == OrderStatus.Created || o.Status == OrderStatus.Confirmed)
            .CountAsync();
    }

    private async Task<List<MonthlyRevenueModel>> GetMonthlyRevenue()
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var monthlyRevenue = await context.Orders.AsNoTracking()
            .Where(o => o.Status == OrderStatus.Shipped
                    && o.CreatedAt.Year == DateTimeOffset.UtcNow.Year)
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
            .Select(g => new MonthlyRevenueModel
            {
                Month = g.Key.Month,
                Revenue = g.Sum(o => o.TotalPrice)
            })
            .ToListAsync();

        return monthlyRevenue;
    }

    private async Task<List<RecentOrderModel>> GetRecentOrdersLast30Days()
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var createdDate = DateTimeOffset.UtcNow.Date.AddDays(-30);

        var recentOrders = await context.Orders.AsNoTracking()
            .Where(o => o.CreatedAt.Date >= createdDate)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new RecentOrderModel
            {
                OrderNumber = o.Number,
                TotalPrice = o.TotalPrice,
                CreatedAt = o.CreatedAt
            })
            .ToListAsync();

        return recentOrders;
    }
}
