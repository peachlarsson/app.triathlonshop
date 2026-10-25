namespace Peach.TriathlonShop.Api.Client.Models;

public class CustomerDto
{
    public required long Id { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

}
