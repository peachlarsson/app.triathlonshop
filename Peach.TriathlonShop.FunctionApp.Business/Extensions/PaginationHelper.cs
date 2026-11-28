using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.FunctionApp.Business.ResponseDtos;

namespace Peach.TriathlonShop.FunctionApp.Business.Extensions;

public static class PaginationHelper
{
    public static async Task<PaginatedResponseDto<T>> PaginateAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than or equal to 1.");

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than or equal to 1.");

        var totalCount = await query.CountAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        if (totalPages > 0 && pageNumber > totalPages)
        {
            pageNumber = totalPages;
        }

        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResponseDto<T>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
