using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.App.Server.Controllers.Models;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.App.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderRowController(TriathlonShopDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<List<OrderRowDto>> Get()
        {
            return await context.OrderRows
                .Select(or => new OrderRowDto
                {
                    Id = or.Id,
                    LineNumber = or.LineNumber,
                    OrderId = or.OrderId,
                    OrderNumber = or.Order.OrderNumber,
                    ArticleId = or.ArticleId,
                    ArticleName = or.Article.Name,
                    UnitPrice = or.Article.UnitPrice,
                    Quantity = or.Quantity
                })
                .ToListAsync();
        }
    }
}
