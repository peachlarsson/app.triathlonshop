using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Peach.TriathlonShop.Api.Contract.Developer.Models;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;
using Peach.TriathlonShop.Database.Models.Enums;
using Peach.TriathlonShop.FunctionApp.Http.Developer.Extensions;
using Peach.TriathlonShop.FunctionApp.Services;
using Peach.TriathlonShop.FunctionApp.Services.Models;
using System.Text.Json;

namespace Peach.TriathlonShop.FunctionApp.Developer
{
    public class DeveloperCreateRandomOrders(ILogger<DeveloperCreateRandomOrders> logger, TriathlonShopDbContext context, SignalRNotifier signalRNotifier)
    {
        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        [Function(nameof(DeveloperCreateRandomOrders))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "developer/orders/random")] HttpRequest req)
        {
            using var reader = new StreamReader(req.Body);
            var requestBody = await reader.ReadToEndAsync();

            var model = JsonSerializer.Deserialize<CreateRandomOrdersModel>(requestBody, jsonSerializerOptions);

            if (model == null)
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            if (!model.IsValidRequestBody())
                return new BadRequestResult();

            var ordersCount = Random.Shared.Next(model.OrdersCountIntervalStart ?? 1, (model.OrdersCountIntervalEnd ?? 10) + 1);

            var existingOrderNumbers = context.Orders.Select(o => o.Number).ToHashSet();
            HashSet<string> orderNumbers = [];

            while (orderNumbers.Count < ordersCount)
            {
                var orderNumber = $"ORD-{Guid.NewGuid():N}"[..16];

                if (existingOrderNumbers.Contains(orderNumber) || !orderNumbers.Add(orderNumber))
                {
                    continue;
                }
            }

            var generatedOrderNumbers = orderNumbers.ToList();

            for (int i = 0; i < ordersCount; i++)
            {
                var placedAt = model.UseRandomPlacedDate ? CreateRandomOrdersExtensions.GetRandomDateTimeOffset() : DateTimeOffset.UtcNow;

                var customer = await GetRandomCustomerAsync();

                /// Pick random destination location
                var destinationLocation = context.Locations
                    .Include(l => l.Country)
                    .Include(l => l.City)
                    .OrderBy(l => Guid.NewGuid())
                    .FirstOrDefault();

                if (destinationLocation == null || destinationLocation.Country == null || destinationLocation.City == null)
                {
                    logger.LogError("No destination location found or missing country/city for the location.");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                /// Pick warehouse that ships to the destination country and has most stock total quantity
                var warehouse = context.Stocks
                    .Where(s => s.Warehouse.WarehouseShipToCountries.Any(wc => wc.CountryId == destinationLocation.Country.Id))
                    .GroupBy(s => s.Warehouse)
                    .Select(g => new
                    {
                        Warehouse = g.Key,
                        TotalStock = g.Sum(s => s.Quantity),
                    })
                    .OrderByDescending(w => w.TotalStock)
                    .FirstOrDefault();

                if (warehouse == null)
                {
                    logger.LogError($"No warehouse found that ships to the destination: {destinationLocation.City.Name} - {destinationLocation.Country.Name} - {destinationLocation.Country.ContinentName}.");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                var order = new Order
                {
                    CustomerId = customer.Id,
                    Number = generatedOrderNumbers[i],
                    Status = model.UseRandomOrderStatus ? CreateRandomOrdersExtensions.GetRandomOrderStatus() : OrderStatus.Created,
                    TotalPrice = 0,
                    DestinationCountryId = destinationLocation.Country.Id,
                    DestinationCityId = destinationLocation.City.Id,
                    DestinationLocationId = destinationLocation.Id,
                    PlacedAt = placedAt,
                };

                /// Pick random articles from the warehouse stock. Quantity > 0.
                /// If the stock quantity is less than the desired quantity, use the available stock quantity.
                int articleCount = Random.Shared.Next(model.ArticlesCountIntervalStart ?? 1, (model.ArticlesCountIntervalEnd ?? 5) + 1);

                /// Get the stock for the selected warehouse and destination location
                var stock = context.Stocks
                    .Include(s => s.Article)
                    .Where(s => s.WarehouseId == warehouse.Warehouse.Id && s.Quantity > 0)
                    .ToList();

                if (stock.Count == 0)
                {
                    logger.LogError("No stock found for the selected warehouse.");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                articleCount = Math.Min(articleCount, stock.Count);

                /// Select random articles from the stock
                var selectedArticles = stock
                    .OrderBy(_ => Random.Shared.Next())
                    .Take(articleCount)
                    .Select(s => s.Article)
                    .ToList();

                int lineNumber = 1;

                foreach (var article in selectedArticles)
                {
                    var articleStock = stock.First(s => s.ArticleId == article.Id);

                    //int quantity = Random.Shared.Next(model.QuantityIntervalStart ?? 1, (model.QuantityIntervalEnd ?? 10) + 1);

                    /// If the stock quantity is less than the desired quantity, use the available stock quantity
                    int requestedQuantity = Random.Shared.Next(model.QuantityIntervalStart ?? 1, (model.QuantityIntervalEnd ?? 10) + 1);

                    int stockQuantity = articleStock.Quantity;

                    int quantity = Math.Min(requestedQuantity, stockQuantity);

                    order.OrderRows.Add(new OrderRow
                    {
                        OrderId = order.Id,
                        ArticleId = article.Id,
                        LineNumber = lineNumber++,
                        Quantity = quantity,
                        RowPrice = article.UnitPrice * quantity,
                        PlacedAt = placedAt,
                    });

                    /// Reduce stock quantities for the selected articles
                    articleStock.Quantity -= quantity;
                }

                order.TotalPrice = order.OrderRows.Sum(or => or.RowPrice);
                context.Orders.Add(order);
                await context.SaveChangesAsync();

                await signalRNotifier.SendHomePageUpdateMessageAsync(SignalRHomePageUpdateType.OrderRows, true);

                if (model.UseRandomDelay)
                {
                    int delayMs = Random.Shared.Next(model.DelayMsIntervalStart ?? 0, model.DelayMsIntervalEnd ?? 3000);
                    await Task.Delay(delayMs);
                }
            }

            return new OkResult();
        }

        private async Task<Customer> GetRandomCustomerAsync()
        {
            bool newCustomer = Random.Shared.Next(2) == 0;
            
            if (context.Customers.Count() == 0 && !newCustomer)
            {
                newCustomer = true;
            }

            bool isCompanyCustomer = Random.Shared.Next(2) == 0;

            if (newCustomer && !isCompanyCustomer)
            {
                var person = new Bogus.Faker().Person;

                var customer = new Customer
                {
                    Name = person.FirstName + " " + person.LastName,
                    Email = person.FirstName.ToLower() + "." + person.LastName.ToLower() + "@private.com",
                };

                context.Customers.Add(customer);
                await context.SaveChangesAsync();

                return customer;
            }
            if (newCustomer && isCompanyCustomer)
            {
                var company = new Bogus.Faker().Company.CompanyName(0);

                var customer = new Customer
                {
                    Name = company,
                    Email = $"sales@{company.Replace(" ", "").ToLower()}.com",
                    IsCompany = true,
                };

                context.Customers.Add(customer);
                await context.SaveChangesAsync();

                return customer;
            }
            else
            {
                var customers = await context.Customers.ToListAsync();
                return customers[Random.Shared.Next(customers.Count)];
            }
        }
    }
}

namespace Peach.TriathlonShop.FunctionApp.Http.Developer.Extensions
{
    public static class CreateRandomOrdersExtensions
    {
        public static bool IsValidRequestBody(this CreateRandomOrdersModel model)
        {
            return model != null;
        }

        public static OrderStatus GetRandomOrderStatus()
        {
            var values = Enum.GetValues(typeof(OrderStatus));

            return (OrderStatus)values.GetValue(Random.Shared.Next(values.Length))!;
        }

        public static DateTimeOffset GetRandomDateTimeOffset()
        {
            var start = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

            var end = DateTimeOffset.UtcNow;

            var range = (end - start).Days;

            var time = new TimeSpan(Random.Shared.Next(0, 24), Random.Shared.Next(0, 60), Random.Shared.Next(0, 60));

            return start.AddDays(Random.Shared.Next(range)).Add(time);
        }
    }
}