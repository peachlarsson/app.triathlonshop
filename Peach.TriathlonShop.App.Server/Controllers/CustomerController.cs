using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.App.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController(TriathlonShopDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<List<Customer>> Get()
        {
            return await context.Customers
                .ToListAsync();
        }
    }
}
