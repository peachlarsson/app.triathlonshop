using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.App.Server.Models;
using System.Linq.Expressions;

namespace Peach.TriathlonShop.App.Server.Utilities
{
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

        public static IQueryable<T> ApplySearch<T>(
            IQueryable<T> query,
            string? searchTerm,
            Expression<Func<T, bool>> searchPredicate)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return query;
            }

            return query.Where(searchPredicate);
        }

        public static IQueryable<T> ApplySort<T>(
            IQueryable<T> query,
            string? sortBy,
            string? sortOrder,
            Dictionary<string, Expression<Func<T, object>>> sortMappings)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(sortBy) || !sortMappings.ContainsKey(sortBy))
            {
                return query;
            }

            var sortExpression = sortMappings[sortBy];
            var isDescending = sortOrder?.ToLower() == "desc";

            return isDescending ? query.OrderByDescending(sortExpression) : query.OrderBy(sortExpression);
        }
    }
}
