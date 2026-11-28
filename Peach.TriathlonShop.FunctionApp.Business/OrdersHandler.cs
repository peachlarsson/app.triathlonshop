using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.FunctionApp.Business.Enums.Common;
using Peach.TriathlonShop.FunctionApp.Business.Enums.Order;
using Peach.TriathlonShop.FunctionApp.Business.Extensions;
using Peach.TriathlonShop.FunctionApp.Business.Queries;
using Peach.TriathlonShop.FunctionApp.Business.ResponseDtos;

namespace Peach.TriathlonShop.FunctionApp.Business;

public class OrdersHandler(TriathlonShopDbContext context)
{
    public async Task<PaginatedResponseDto<OrdersResponseDto>> GetOrdersAsync(GetOrdersQuery request)
    {
        var query = context.Orders
            .AsNoTracking();

        if (request.Id.HasValue)
        {
            query = query.Where(c => c.Id == request.Id.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.OrderNumber))
        {
            var term = $"%{request.OrderNumber.Trim()}%";
            query = query.Where(c => c.Number != null && EF.Functions.Like(c.Number, term));
        }

        if (request.OrderStatuses != null && request.OrderStatuses.Count > 0)
        {
            query = query.Where(c => request.OrderStatuses.Contains(c.Status));
        }

        if (!string.IsNullOrWhiteSpace(request.ArticleName))
        {
            var term = $"%{request.ArticleName.Trim()}%";
            query = query.Where(c => c.OrderRows.Any(or => or.Article != null && EF.Functions.Like(or.Article.ArticleName, term)));
        }

        if (!string.IsNullOrWhiteSpace(request.CustomerName))
        {
            var term = $"%{request.CustomerName.Trim()}%";
            query = query.Where(c => c.Customer!.Name != null && EF.Functions.Like(c.Customer.Name, term));
        }

        if (!string.IsNullOrWhiteSpace(request.DestinationCountry))
        {
            var term = $"%{request.DestinationCountry.Trim()}%";
            query = query.Where(c => c.DestinationLocation.Country.Name != null && EF.Functions.Like(c.DestinationLocation.Country.Name, term));
        }

        if (!string.IsNullOrWhiteSpace(request.DestinationCity))
        {
            var term = $"%{request.DestinationCity.Trim()}%";
            query = query.Where(c => c.DestinationLocation.City.Name != null && EF.Functions.Like(c.DestinationLocation.City.Name, term));
        }


        if (!string.IsNullOrWhiteSpace(request.DestinationLocation))
        {
            var term = $"%{request.DestinationLocation.Trim()}%";
            query = query.Where(c => c.DestinationLocation.City.Name != null && EF.Functions.Like(c.DestinationLocation.City.Name, term) ||
                                     c.DestinationLocation.Country.Name != null && EF.Functions.Like(c.DestinationLocation.Country.Name, term));
        }

        query = request.SortKey switch
        {
            SortKey.OrderNumber => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.Number)
                : query.OrderByDescending(q => q.Number),
            SortKey.CustomerName => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.Customer!.Name)
                : query.OrderByDescending(q => q.Customer!.Name),
            SortKey.OrderStatus => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.Status)
                : query.OrderByDescending(q => q.Status),
            SortKey.TotalPrice => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.OrderRows.Sum(or => or.RowPrice))
                : query.OrderByDescending(q => q.OrderRows.Sum(or => or.RowPrice)),
            SortKey.TransportStatus => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.TransportOrder!.Status)
                : query.OrderByDescending(q => q.TransportOrder!.Status),
            SortKey.EstimatedDeliveryDate => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.TransportOrder != null ? q.TransportOrder.EstimatedDeliveryDate : null)
                : query.OrderByDescending(q => q.TransportOrder != null ? q.TransportOrder.EstimatedDeliveryDate : null),
            SortKey.PlacedAt => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.PlacedAt)
                : query.OrderByDescending(q => q.PlacedAt),
            SortKey.CreatedAt => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.CreatedAt)
                : query.OrderByDescending(q => q.CreatedAt),
            SortKey.UpdatedAt => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.UpdatedAt)
                : query.OrderByDescending(q => q.UpdatedAt),
            _ => request.SortDirection == SortDirection.Asc
                ? query.OrderBy(q => q.Id)
                : query.OrderByDescending(q => q.Id)
        };

        var queryDto = query.Select(c => new OrdersResponseDto
        {
            Id = c.Id,
            OrderNumber = c.Number,
            Status = c.Status.ToString(),
            DestinationLocation = new LocationDto
            {
                CountryCode = c.DestinationLocation.Country.Code,
                CountryName = c.DestinationLocation.Country.Name,
                CityName = c.DestinationLocation.City.Name,
            },
            Customer = !request.IncludeCustomer ? null : new CustomerDto
            {
                Id = c.Customer!.Id,
                Email = c.Customer.Email,
                Name = c.Customer.Name,
                CreatedAt = c.Customer.CreatedAt,
                UpdatedAt = c.Customer.UpdatedAt
            },
            OrderRows = !request.IncludeOrderRows ? new() : c.OrderRows.Select(or => new OrderRowDto
            {
                Id = or.Id,
                OrderId = or.OrderId,
                OrderNumber = or.Order!.Number,
                LineNumber = or.LineNumber,
                ArticleId = or.ArticleId,
                ArticleName = or.Article!.ArticleName,
                UnitPrice = or.Article!.UnitPrice,
                Quantity = or.Quantity,
                RowPrice = or.RowPrice,
                CustomerId = or.Order.CustomerId,
                CustomerName = or.Order!.Customer!.Name,
                PlacedAt = or.PlacedAt,
                CreatedAt = or.CreatedAt,
                UpdatedAt = or.UpdatedAt
            }).ToList(),
            Notifications = c.Notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                OrderId = n.OrderId,
                OrderStatus = n.OrderStatus,
                Destination = n.Destination,
                CreatedAt = n.CreatedAt
            }).ToList(),
            TransportOrder = c.TransportOrder == null ? null : new TransportOrderDto
            {
                Id = c.TransportOrder.Id,
                ConsignmentNumber = c.TransportOrder.ConsignmentNumber,
                TransportOrderStatus = c.TransportOrder.Status.ToString(),
                Carrier = c.TransportOrder.Carrier,
                Destination = c.TransportOrder.Destination,
                ShippedAt = c.TransportOrder.ShippedAt,
                EstimatedDeliveryDate = c.TransportOrder.EstimatedDeliveryDate,
            },
            //TransportEvents
            Invoices = c.Invoices.Select(i => new InvoiceDto
            {
                Id = i.Id,
                OrderId = i.OrderId,
                Status = i.Status,
                CreatedAt = i.CreatedAt
            }).ToList(),
            TotalPrice = c.OrderRows.Sum(or => or.RowPrice),
            PlacedAt = c.PlacedAt,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });

        return await queryDto.PaginateAsync(request.PageNumber, request.PageSize);
    }
}
