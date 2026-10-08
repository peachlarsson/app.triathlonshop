using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.Database;

public class TriathlonShopDbContext(DbContextOptions<TriathlonShopDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
}