using Microsoft.AspNetCore.Mvc;
using Peach.TriathlonShop.App.Server.Controllers.Enums.Article;
using Peach.TriathlonShop.App.Server.Controllers.Enums.Common;
using Peach.TriathlonShop.App.Server.Models;
using Peach.TriathlonShop.App.Server.Utilities;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController(TriathlonShopDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<Article>>> GetPaginated(
            SortKey sortBy = SortKey.Id,
            SortDirection sortOrder = SortDirection.Asc,
            int pageNumber = 1,
            int pageSize = 100,
            string? name = null)
        {
            var query = context.Articles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(q => q.Name.ToLower().Contains(name.ToLower()));
            }

            query = sortBy switch
            {
                SortKey.Number => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.Number)
                    : query.OrderByDescending(q => q.Number),
                SortKey.Name => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.Name)
                    : query.OrderByDescending(q => q.Name),
                SortKey.UnitPrice => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.UnitPrice)
                    : query.OrderByDescending(q => q.UnitPrice),
                SortKey.CreatedAt => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.CreatedAt)
                    : query.OrderByDescending(q => q.CreatedAt),
                SortKey.UpdatedAt => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.UpdatedAt)
                    : query.OrderByDescending(q => q.UpdatedAt),
                _ => sortOrder == SortDirection.Asc
                    ? query.OrderBy(q => q.Id)
                    : query.OrderByDescending(q => q.Id)
            };

            var paginatedResult = await PaginationHelper.PaginateAsync(query, pageNumber, pageSize);
            return Ok(paginatedResult);
        }
    }
}
