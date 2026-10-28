using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Peach.TriathlonShop.Api.Contract.Enums.Article;
using Peach.TriathlonShop.Api.Contract.Enums.Common;
using Peach.TriathlonShop.Api.Contract.Models.Stock;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.FunctionApp.Utilities;

namespace Peach.TriathlonShop.FunctionApp.Http.Stock;

public class GetArticles(TriathlonShopDbContext context)
{
    [Function(nameof(GetArticles))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "stock/articles")] HttpRequest req)
    {
        var queryParams = req.Query;
        
        var queryParam = new GetArticlesRequestDto
        {
            ArticleName = queryParams["articleName"],
            SortKey = Enum.TryParse<SortKey>(queryParams["sortBy"], out var sortBy) ? sortBy : SortKey.Name,
            SortDirection = Enum.TryParse<SortDirection>(queryParams["sortOrder"], out var sortOrder) ? sortOrder : SortDirection.Asc,
            PageNumber = int.TryParse(queryParams["pageNumber"], out var pageNumber) ? pageNumber : 1,
            PageSize = int.TryParse(queryParams["pageSize"], out var pageSize) ? pageSize : 100
        };

        var query = context.Articles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryParam.ArticleName))
        {
            query = query.Where(q => q.Name.ToLower().Contains(queryParam.ArticleName.ToLower()));
        }

        query = queryParam.SortKey switch
        {
            SortKey.Number => queryParam.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.Number)
                : query.OrderByDescending(q => q.Number),
            SortKey.Name => queryParam.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.Name)
                : query.OrderByDescending(q => q.Name),
            SortKey.UnitPrice => queryParam.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.UnitPrice)
                : query.OrderByDescending(q => q.UnitPrice),
            SortKey.CreatedAt => queryParam.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.CreatedAt)
                : query.OrderByDescending(q => q.CreatedAt),
            SortKey.UpdatedAt => queryParam.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.UpdatedAt)
                : query.OrderByDescending(q => q.UpdatedAt),
            _ => queryParam.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.Id)
                : query.OrderByDescending(q => q.Id)
        };

        var articleQuery = query
            .Select(a => new ArticleDto
            {
                Id = a.Id,
                Name = a.Name,
                Number = a.Number,
                UnitPrice = a.UnitPrice,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            });

        var paginatedResult = await PaginationHelper.PaginateAsync(articleQuery, queryParam.PageNumber, queryParam.PageSize);
        return new OkObjectResult(paginatedResult);
    }
}
