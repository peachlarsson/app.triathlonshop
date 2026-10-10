using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.App.Server.Models;
using Peach.TriathlonShop.App.Server.Utilities;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.App.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController(TriathlonShopDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<List<Order>> Get()
        {
            return await context.Orders
                .ToListAsync();
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<PaginatedResponse<Order>>> GetPaginated(
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than 0");
            }

            var query = context.Orders.AsQueryable();
            var paginatedResult = await PaginationHelper.PaginateAsync(query, pageNumber, pageSize);
            return Ok(paginatedResult);
        }
    }
}

