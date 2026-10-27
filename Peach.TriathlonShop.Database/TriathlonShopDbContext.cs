using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.Database;

public class TriathlonShopDbContext(DbContextOptions<TriathlonShopDbContext> options) : DbContext(options)
{
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderRow> OrderRows => Set<OrderRow>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<TransportOrder> TransportOrders => Set<TransportOrder>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<OrderEvent> OrderEvents => Set<OrderEvent>();

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Route> Routes => Set<Route>();

    private const string DefaultSchema = "dbo";
    private const string DefaultTimestampSql = "SYSDATETIMEOFFSET()";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Article>(entity =>
        {
            entity.ToTable(nameof(Article), DefaultSchema);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql(DefaultTimestampSql);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable(nameof(Customer), DefaultSchema);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql(DefaultTimestampSql);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("SalesOrder", DefaultSchema);

            entity.Property(e => e.Status)
                  .HasConversion<string>();

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql(DefaultTimestampSql);
        });

        modelBuilder.Entity<OrderRow>(entity =>
        {
            entity.ToTable("SalesOrderRow", DefaultSchema);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql(DefaultTimestampSql);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("CustomerNotification", DefaultSchema);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql(DefaultTimestampSql);
        });

        modelBuilder.Entity<TransportOrder>(entity =>
        {
            entity.ToTable(nameof(TransportOrder), DefaultSchema);

            entity.Property(e => e.Status)
                  .HasConversion<string>();

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql(DefaultTimestampSql);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable(nameof(Invoice), DefaultSchema);

            entity.Property(e => e.Status)
                  .HasConversion<string>();

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql(DefaultTimestampSql);
        });

        modelBuilder.Entity<OrderEvent>(entity =>
        {
            entity.ToTable(nameof(OrderEvent), DefaultSchema);

            entity.Property(e => e.Status)
                  .HasConversion<string>();

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql(DefaultTimestampSql);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable(nameof(Warehouse), DefaultSchema);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql(DefaultTimestampSql);
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.ToTable(nameof(Stock), DefaultSchema);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql(DefaultTimestampSql);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable(nameof(Location), DefaultSchema);
            
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql(DefaultTimestampSql);
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.ToTable(nameof(Route), DefaultSchema);

            entity.Property(e => e.RouteStatus)
                  .HasConversion<string>();

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql(DefaultTimestampSql);
        });
    }
}