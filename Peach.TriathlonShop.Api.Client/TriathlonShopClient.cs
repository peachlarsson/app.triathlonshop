using Microsoft.Extensions.Configuration;
using Peach.TriathlonShop.Api.Contract.Developer.Models;
using Peach.TriathlonShop.Api.Contract.Models;
using Peach.TriathlonShop.Api.Contract.Models.Orders;
using Peach.TriathlonShop.Api.Contract.Models.Stock;
using System.Text;
using System.Text.Json;

namespace Peach.TriathlonShop.Api.Client;

public class TriathlonShopClient(HttpClient httpClient, IConfiguration configuration) : ITriathlonShopClient
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task<Response<SignalRConnectionInfo>> GetNegotiationAsync(string hubName)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"negotiation/{hubName}", UriKind.Relative)
        };

        var response = await httpClient.SendAsync(request);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new Response<SignalRConnectionInfo>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = $"responseContent: {responseContent}"
            };
        }

        return new Response<SignalRConnectionInfo>
        {
            IsSuccess = true,
            StatusCode = (int)response.StatusCode,
            Content = JsonSerializer.Deserialize<SignalRConnectionInfo>(responseContent, jsonSerializerOptions)
        };
    }

    public async Task<Response<PaginatedResponse<CustomerDto>>> GetCustomerAsync(GetCustomersRequest request)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "customerName", request.CustomerName ?? string.Empty },
            { "pageNumber", request.PageNumber.ToString() },
            { "pageSize", request.PageSize.ToString() },
            { "sortKey", request.SortKey.ToString() },
            { "sortDirection", request.SortDirection.ToString() }
        };

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"customers?{queryString}", UriKind.Relative)
        };

        var response = await httpClient.SendAsync(httpRequest);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new Response<PaginatedResponse<CustomerDto>>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = responseContent,
            };
        }

        return new Response<PaginatedResponse<CustomerDto>>
        {
            IsSuccess = true,
            StatusCode = (int)response.StatusCode,
            Content = JsonSerializer.Deserialize<PaginatedResponse<CustomerDto>>(responseContent, jsonSerializerOptions),
        };
    }

    #region Orders
    public async Task<Response<PaginatedResponse<OrderResponseDto>>> GetOrdersAsync(GetOrdersRequestDto request)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "id", request.Id?.ToString() ?? string.Empty },
            { "orderNumber", request.OrderNumber ?? string.Empty },
            { "customerName", request.CustomerName ?? string.Empty },
            { "articleName", request.ArticleName ?? string.Empty },
            { "pageNumber", request.PageNumber.ToString() },
            { "pageSize", request.PageSize.ToString() },
            { "sortKey", request.SortKey.ToString() },
            { "sortDirection", request.SortDirection.ToString() }
        };

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"orders?{queryString}", UriKind.Relative)
        };

        var response = await httpClient.SendAsync(httpRequest);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new Response<PaginatedResponse<OrderResponseDto>>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = responseContent,
            };
        }

        return new Response<PaginatedResponse<OrderResponseDto>>
        {
            IsSuccess = true,
            StatusCode = (int)response.StatusCode,
            Content = JsonSerializer.Deserialize<PaginatedResponse<OrderResponseDto>>(responseContent, jsonSerializerOptions),
        };
    }

    public async Task<Response<string>> CreateOrderAsync(CreateOrderRequestDto request)
    {
        var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("orders/create", UriKind.Relative),
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(httpRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            return new Response<string>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = responseContent,
            };
        }
        
        return new Response<string>
        {
            IsSuccess = true,
            StatusCode = (int)response.StatusCode,
            Content = responseContent
        };
    }

    #endregion
    public async Task<Response<string>> SetOrderConfirm(long orderId)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"order/{orderId}/confirm", UriKind.Relative)
        };

        var response = await httpClient.SendAsync(request);

        var responseContent = await response.Content.ReadAsStringAsync();

        return new Response<string>
        {
            IsSuccess = response.IsSuccessStatusCode,
            StatusCode = (int)response.StatusCode,
            Content = responseContent,
            ErrorMessage = response.IsSuccessStatusCode ? null : responseContent,
        };
    }

    public async Task<Response<string>> SetOrderShipped(long orderId)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"order/{orderId}/ship", UriKind.Relative)
        };

        var response = await httpClient.SendAsync(request);

        var responseContent = await response.Content.ReadAsStringAsync();

        return new Response<string>
        {
            IsSuccess = response.IsSuccessStatusCode,
            StatusCode = (int)response.StatusCode,
            Content = responseContent,
            ErrorMessage = response.IsSuccessStatusCode ? null : responseContent,
        };
    }

    public async Task<Response<DashboardResponseModel>> GetDashboardDataAsync()
    {
        var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("dashboard", UriKind.Relative)
        };

        var response = await httpClient.SendAsync(httpRequest);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new Response<DashboardResponseModel>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = responseContent,
            };
        }

        return new Response<DashboardResponseModel>
        {
            IsSuccess = true,
            StatusCode = (int)response.StatusCode,
            Content = JsonSerializer.Deserialize<DashboardResponseModel>(responseContent, jsonSerializerOptions),
        };
    }

    public async Task<Response<GetAllWarehouseStockDto>> GetAllWarehouseStockAsync()
    {
        var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("stock/all-warehouses", UriKind.Relative)
        };

        var response = await httpClient.SendAsync(httpRequest);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new Response<GetAllWarehouseStockDto>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = responseContent,
            };
        }

        return new Response<GetAllWarehouseStockDto>
        {
            IsSuccess = true,
            StatusCode = (int)response.StatusCode,
            Content = JsonSerializer.Deserialize<GetAllWarehouseStockDto>(responseContent, jsonSerializerOptions),
        };
    }

    public async Task<Response<PaginatedResponse<ArticleDto>>> GetArticlesAsync(GetArticlesRequestDto request)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "articleName", request.ArticleName ?? string.Empty },
            { "pageNumber", request.PageNumber.ToString() },
            { "pageSize", request.PageSize.ToString() },
            { "sortBy", request.SortKey.ToString() },
            { "sortOrder", request.SortDirection.ToString() }
        };

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        
        var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"stock/articles?{queryString}", UriKind.Relative)
        };
        
        var response = await httpClient.SendAsync(httpRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            return new Response<PaginatedResponse<ArticleDto>>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = responseContent,
            };
        }
        
        return new Response<PaginatedResponse<ArticleDto>>
        {
            IsSuccess = true,
            StatusCode = (int)response.StatusCode,
            Content = JsonSerializer.Deserialize<PaginatedResponse<ArticleDto>>(responseContent, jsonSerializerOptions),
        };
    }

    public async Task<Response<string>> CreateRandomOrdersAsync(CreateRandomOrdersModel request)
    {
        var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("developer/orders/random", UriKind.Relative),
            Content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(httpRequest);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new Response<string>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = responseContent,
            };
        }

        return new Response<string>
        {
            IsSuccess = true,
            StatusCode = (int)response.StatusCode,
            Content = responseContent
        };
    }
}

public class SignalRConnectionInfo
{
    public required string Url { get; set; }
    public required string AccessToken { get; set; }
}