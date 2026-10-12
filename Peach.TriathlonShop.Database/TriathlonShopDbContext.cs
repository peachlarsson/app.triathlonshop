using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.Database;

public class TriathlonShopDbContext(DbContextOptions<TriathlonShopDbContext> options) : DbContext(options)
{
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderRow> OrderRows => Set<OrderRow>();

    private const string DefaultSchema = "dbo";
    private const string DefaultTimestampSql = "SYSDATETIMEOFFSET()";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Article>()
            .ToTable(nameof(Article), DefaultSchema)
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql(DefaultTimestampSql);

        modelBuilder.Entity<Customer>()
            .ToTable(nameof(Customer), DefaultSchema)
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql(DefaultTimestampSql);

        modelBuilder.Entity<Order>()
            .ToTable("SalesOrder", DefaultSchema)
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql(DefaultTimestampSql);

        modelBuilder.Entity<OrderRow>()
            .ToTable("SalesOrderRow", DefaultSchema)
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql(DefaultTimestampSql);
    }
}