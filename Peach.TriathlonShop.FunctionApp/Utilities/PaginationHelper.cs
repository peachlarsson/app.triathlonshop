using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.FunctionApp.Utilities.Models;

namespace Peach.TriathlonShop.FunctionApp.Utilities;

public static class PaginationHelper
{
    public static async Task<PaginatedResponse<T>> PaginateAsync<T>(
        IQueryable<T> query,
        int pageNumber,
        int pageSize)
        where T : class
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than or equal to 1.");

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        if (totalPages > 0 && pageNumber > totalPages)
        {
            pageNumber = totalPages;
        }

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
