using Peach.TriathlonShop.Api.Contract.Enums.Common;
using Peach.TriathlonShop.Api.Contract.Enums.Article;

namespace Peach.TriathlonShop.Api.Contract.Models.Stock;

public class GetArticlesRequestDto
{
    public string? ArticleName { get; set; }
    public SortKey SortBy { get; set; } = SortKey.Name;
    public SortDirection SortOrder { get; set; } = SortDirection.Asc;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 100;
}