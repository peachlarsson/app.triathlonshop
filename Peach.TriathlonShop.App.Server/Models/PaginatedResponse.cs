namespace Peach.TriathlonShop.App.Server.Models;

public class PaginatedResponse<T>
{
    public required List<T> Data { get; set; }
    public required int TotalCount { get; set; }
    public decimal? TotalPrice { get; set; }
    public required int PageNumber { get; set; }
    public required int PageSize { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
}
