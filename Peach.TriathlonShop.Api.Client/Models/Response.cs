namespace Peach.TriathlonShop.Api.Client.Models;

public class Response<T>
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public T? Content { get; set; }
    public string? ErrorMessage { get; set; }
}
