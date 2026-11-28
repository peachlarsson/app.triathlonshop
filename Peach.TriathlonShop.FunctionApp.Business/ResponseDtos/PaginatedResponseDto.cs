using System.ComponentModel;

namespace Peach.TriathlonShop.FunctionApp.Business.ResponseDtos;

public class PaginatedResponseDto<T>
{
    public required List<T> Data { get; set; }
    public required int TotalCount { get; set; }
    public decimal? TotalPrice { get; set; }

    [Description("Starts from 1.")]
    public required int PageNumber { get; set; }
    public required int PageSize { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
}
