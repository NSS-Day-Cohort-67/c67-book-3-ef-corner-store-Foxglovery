using Microsoft.EntityFrameworkCore;
using CornerStore.Models;
public class CornerStoreDbContext : DbContext
{
    public DbSet<Cashier> Cashiers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<Category> Categories { get; set; }
    

    public CornerStoreDbContext(DbContextOptions<CornerStoreDbContext> context) : base(context)
    {

    }

    //allows us to configure the schema when migrating as well as seed data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cashier>().HasData(new Cashier[]
        {
            new Cashier {Id = 1, FirstName = "Groogery", LastName = "Adlerburb"},
            new Cashier {Id = 2, FirstName = "Shlebethany", LastName = "Jerp"},
            new Cashier {Id = 3, FirstName = "Nikotela", LastName = "Demus"},
            new Cashier {Id = 4, FirstName = "Sasparilla", LastName = "Montegue"},
            new Cashier {Id = 5, FirstName = "Juniper", LastName = "Cuddlefash"}
        });

        modelBuilder.Entity<Category>().HasData(new Category[]
        {
            new Category {Id = 1, CategoryName = "Cosmetics"},
            new Category {Id = 2, CategoryName = "Grocery"},
            new Category {Id = 3, CategoryName = "Relics"},
            new Category {Id = 4, CategoryName = "Decor"},
            new Category {Id = 5, CategoryName = "Electronics"}
        });

        modelBuilder.Entity<Product>().HasData(new Product[]
        {
            new Product {Id = 1, ProductName = "Brip-Brap-Braids", Price = 75M, Brand = "Acceptable Value", CategoryId = 1},
            new Product {Id = 2, ProductName = "Scottish Breakfast Kit", Price = 23M, Brand = "Distinctly Caledonian", CategoryId = 2},
            new Product {Id = 3, ProductName = "Feather-Duster Lamp", Price = 125M, Brand = "Niche Cliche", CategoryId = 4},
            new Product {Id = 4, ProductName = "St. Jorbin's Pinky Knuckle", Price = 2001M, Brand = "Homo-Sapien", CategoryId = 3},
            new Product {Id = 5, ProductName = "Talkboy", Price = 600M, Brand = "Walkman", CategoryId = 5},
            new Product {Id = 6, ProductName = "Plate-O-Fire", Price = 7M, Brand = "Doughnut Eat", CategoryId = 2},
            new Product {Id = 7, ProductName = "Harambe's Blanket", Price = 3500M, Brand = "Room Essentials", CategoryId = 3},
            new Product {Id = 8, ProductName = "Full Ipod Mini", Price = 60M, Brand = "Apple", CategoryId = 5}
        });

        modelBuilder.Entity<Order>().HasData(new Order[]
        {
            new Order {Id = 1, CashierId = 1, PaidOnDate = new DateTime(2024, 1, 12)},
            new Order {Id = 2, CashierId = 5, PaidOnDate = new DateTime(2024, 1, 13)},
            new Order {Id = 3, CashierId = 2, PaidOnDate = new DateTime(2024, 1, 14)},
            new Order {Id = 4, CashierId = 4, PaidOnDate = new DateTime(2024, 1, 11)},
            new Order {Id = 5, CashierId = 3, PaidOnDate = new DateTime(2024, 1, 10)},
          
        });

        modelBuilder.Entity<OrderProduct>().HasData(new OrderProduct[]
        {
           new OrderProduct {Id = 1, OrderId = 1, ProductId = 2, Quantity = 1},
           new OrderProduct {Id = 2, OrderId = 1, ProductId = 4, Quantity = 1},
           new OrderProduct {Id = 3, OrderId = 2, ProductId = 1, Quantity = 1},
           new OrderProduct {Id = 4, OrderId = 2, ProductId = 4, Quantity = 1},
           new OrderProduct {Id = 5, OrderId = 3, ProductId = 6, Quantity = 1},
           new OrderProduct {Id = 6, OrderId = 4, ProductId = 7, Quantity = 1},
           new OrderProduct {Id = 7, OrderId = 5, ProductId = 5, Quantity = 1}
        });

    }
}