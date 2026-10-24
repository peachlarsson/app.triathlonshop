using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.App.shadcn.Server.Utilities.Models;

namespace Peach.TriathlonShop.App.shadcn.Server.Utilities;

public static class PaginationHelper
{
    public static async Task<PaginatedResponse<T>> PaginateAsync<T>(
        IQueryable<T> query,
        int pageNumber,
        int pageSize)
        where T : class
    {
        var totalCount = await query.CountAsync();

        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<T>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
