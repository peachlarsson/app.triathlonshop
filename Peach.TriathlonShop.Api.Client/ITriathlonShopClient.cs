using Peach.TriathlonShop.Api.Contract.Models;

namespace Peach.TriathlonShop.Api.Client;

public interface ITriathlonShopClient
{
    public Task<Response<SignalRConnectionInfo>> GetNegotiationAsync(string hubName);
    public Task<Response<PaginatedResponse<CustomerDto>>> GetCustomerAsync(GetCustomersRequest request);
    public Task<Response<PaginatedResponse<OrderResponseDto>>> GetOrdersAsync(GetOrdersRequestDto request);
    public Task<Response<string>> SetOrderConfirm(long orderId);
    public Task<Response<string>> SetOrderShipped(long orderId);
    public Task<Response<DashboardResponseModel>> GetDashboardDataAsync();
}
