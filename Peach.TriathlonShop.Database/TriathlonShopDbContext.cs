using Microsoft.EntityFrameworkCore;
using Peach.TriathlonShop.Database.Models;

namespace Peach.TriathlonShop.Database;
/*
Skriv ett program som kan skapa några ordrar med orderrader och artiklar. 
Gör sedan en sökfunktion där användaren väljer en artikel, 
och får ut en lista på alla orderrader där artikeln ingår.

• En order innehåller ett kundnamn, och minst en orderrad.

• En orderrad innehåller ett radnummer, ett antal (av en artikel), 
  och pekar på en artikel. Den tillhör alltid en order.

• En artikel innehåller ett artikelnummer, artikelnamn och ett styckpris. 
  En artikel kan finnas på flera orderrader, men måste inte finnas på någon.
  En orderrad har alltid en artikel, och kan inte ha fler än en.

Skapandet av ordrar, orderrader och artiklar kan antingen göras med UI där användaren matar in data, 
men det är också ok att skriva kod som genererar motsvarande data.

Sökfunktionen ska ha ett användargränssnitt, där användaren på något sätt anger en artikel. 
Resultatet ska visa en lista på de orderrader som artikeln finns i, och där bör visas ordernummer, 
kundnamn, orderrad, styckpris, antal och summa pris för orderraden. 
Man ska också kunna se hur många rader som hittades, och summan av ordervärdet för dessa orderrader.
 */
public class TriathlonShopDbContext(DbContextOptions<TriathlonShopDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderRow> OrderRows => Set<OrderRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .ToTable(nameof(Product), schema: "dbo");

        modelBuilder.Entity<Customer>()
            .ToTable(nameof(Customer), schema: "dbo");

        modelBuilder.Entity<Order>()
            .ToTable("SalesOrder", schema: "dbo");

        modelBuilder.Entity<OrderRow>()
            .ToTable("SalesOrderRow", schema: "dbo");
    }
}