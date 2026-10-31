using Peach.TriathlonShop.Api.Contract.Developer.Models;
using Peach.TriathlonShop.Api.Contract.Models;
using Peach.TriathlonShop.Api.Contract.Models.Orders;
using Peach.TriathlonShop.Api.Contract.Models.Stock;

namespace Peach.TriathlonShop.Api.Client;

public interface ITriathlonShopClient
{
    public Task<Response<SignalRConnectionInfo>> GetNegotiationAsync(string hubName);
    public Task<Response<PaginatedResponse<CustomerDto>>> GetCustomerAsync(GetCustomersRequest request);

    #region Orders
    public Task<Response<PaginatedResponse<OrderResponseDto>>> GetOrdersAsync(GetOrdersRequestDto request);
    public Task<Response<string>> CreateOrderAsync(CreateOrderRequestDto request);
    #endregion

    public Task<Response<string>> SetOrderConfirm(long orderId);
    public Task<Response<string>> SetOrderShipped(long orderId);
    public Task<Response<DashboardResponseModel>> GetDashboardDataAsync();
    public Task<Response<GetAllWarehouseStockDto>> GetAllWarehouseStockAsync();
    public Task<Response<PaginatedResponse<ArticleDto>>> GetArticlesAsync(GetArticlesRequestDto request);
    public Task<Response<string>> CreateRandomOrdersAsync(CreateRandomOrdersModel request);
}
