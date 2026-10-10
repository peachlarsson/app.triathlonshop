using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.App.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleController(TriathlonShopDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<List<Article>> Get()
        {
            return await context.Articles
                .ToListAsync();
        }
    }
}
