using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
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
    public class DeveloperCreateRandomOrders(TriathlonShopDbContext context, SignalRNotifier signalRNotifier)
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
                var customer = await GetRandomCustomerAsync();

                var destinationCity = context.Cities.OrderBy(c => Guid.NewGuid()).FirstOrDefault();

                var order = new Order
                {
                    CustomerId = customer.Id,
                    Number = generatedOrderNumbers[i],
                    Status = model.UseRandomOrderStatus ? CreateRandomOrdersExtensions.GetRandomOrderStatus() : OrderStatus.Created,
                    TotalPrice = 0,
                    DestinationCountryId = destinationCity?.CountryId ?? 0,
                    DestinationCityId = destinationCity?.Id ?? 0,
                    DestinationLocationId = context.Locations.OrderBy(l => Guid.NewGuid()).FirstOrDefault()?.Id ?? 0,
                    PlacedAt = model.UseRandomPlacedDate ? CreateRandomOrdersExtensions.GetRandomDateTimeOffset() : DateTimeOffset.UtcNow,
                };

                int articleCount = Random.Shared.Next(model.ArticlesCountIntervalStart ?? 1, (model.ArticlesCountIntervalEnd ?? 5) + 1);

                var articles = context.Articles.ToList();

                articleCount = Math.Min(articleCount, articles.Count);

                var selectedArticles = articles
                    .OrderBy(_ => Random.Shared.Next())
                    .Take(articleCount)
                    .ToList();

                int lineNumber = 1;

                foreach (var article in selectedArticles)
                {
                    int quantity = Random.Shared.Next(model.QuantityIntervalStart ?? 1, (model.QuantityIntervalEnd ?? 10) + 1);

                    order.OrderRows.Add(new OrderRow
                    {
                        OrderId = order.Id,
                        ArticleId = article.Id,
                        LineNumber = lineNumber++,
                        Quantity = quantity,
                        RowPrice = article.UnitPrice * quantity,
                    });
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

            return start.AddDays(Random.Shared.Next(range));
        }
    }
}