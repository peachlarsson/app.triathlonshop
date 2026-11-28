using Peach.TriathlonShop.Database.Models.Enums;

namespace Peach.TriathlonShop.FunctionApp.Business.ResponseDtos;

public class NotificationDto
{
    public required long Id { get; set; }
    public required long OrderId { get; set; }
    public required OrderStatus OrderStatus { get; set; }
    public required string Destination { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}
