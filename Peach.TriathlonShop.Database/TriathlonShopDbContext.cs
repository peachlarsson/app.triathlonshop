using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.Database;

public class TriathlonShopDbContext(DbContextOptions<TriathlonShopDbContext> options) : DbContext(options)
{
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderRow> OrderRows => Set<OrderRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Article>()
            .ToTable(nameof(Article), schema: "dbo");

        modelBuilder.Entity<Customer>()
            .ToTable(nameof(Customer), schema: "dbo");

        modelBuilder.Entity<Order>()
            .ToTable("SalesOrder", schema: "dbo");

        modelBuilder.Entity<OrderRow>()
            .ToTable("SalesOrderRow", schema: "dbo");
    }
}