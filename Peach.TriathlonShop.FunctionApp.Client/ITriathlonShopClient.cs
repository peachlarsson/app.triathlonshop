using Peach.TriathlonShop.FunctionApp.Client.Models;

namespace Peach.TriathlonShop.FunctionApp.Client;

public interface ITriathlonShopClient
{
    public Task<Response<SignalRConnectionInfo>> GetNegotiationAsync(string hubName);
    public Task<Response<PaginatedResponse<CustomerDto>>> GetCustomerAsync(GetCustomersRequest request);
    public Task<Response<string>> SetOrderConfirm(long orderId);
    public Task<Response<string>> SetOrderShipped(long orderId);
}
