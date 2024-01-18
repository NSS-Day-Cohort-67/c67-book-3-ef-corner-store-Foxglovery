using CornerStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using CornerStore.Models.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core and provides dummy value for testing
builder.Services.AddNpgsql<CornerStoreDbContext>(builder.Configuration["CornerStoreDbConnectionString"] ?? "testing");

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/cashiers", (CornerStoreDbContext db, Cashier newCashier) =>
{
    db.Cashiers.Add(newCashier);
    db.SaveChanges();
    return Results.Created($"/api/cashiers/{newCashier.Id}", newCashier);
});

app.MapGet("/api/cashiers/{id}", (CornerStoreDbContext db, int id) =>
{
    var cashierDto = db.Cashiers
        .Where(c => c.Id == id)
        .Include(c => c.Orders)
            .ThenInclude(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                    .ThenInclude(p => p.Category)
        .Select(c => new CashierDTO
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Orders = c.Orders.Select(o => new OrderDTO
            {
                Id = o.Id,
                CashierId = o.CashierId,
                OrderProducts = o.OrderProducts.Select(op => new OrderProductDTO
                {
                    Id = op.Id,
                    ProductId = op.ProductId,
                    OrderId = op.OrderId,
                    Quantity = op.Quantity,
                    Product = new ProductDTO
                    {
                        Id = op.Product.Id,
                        ProductName = op.Product.ProductName,
                        Price = op.Product.Price,
                        Brand = op.Product.Brand,
                        CategoryId = op.Product.CategoryId,
                        Category = new CategoryDTO
                        {
                            Id = op.Product.Category.Id,
                            CategoryName = op.Product.Category.CategoryName
                        }
                        
                    }
                }).ToList()
                
            }).ToList()
        })
        .FirstOrDefault();

    if (cashierDto == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(cashierDto);
});

//return all products with categories
// should include an optional query for category name or product name
app.MapGet("/api/products", (CornerStoreDbContext db, string? categoryName, string? productName) =>
{
    var query = db.Products
    .Include(p => p.Category)
    .AsQueryable();

    var lowerProductName = productName?.ToLower();
    var lowerCatName = categoryName?.ToLower();
    if (!string.IsNullOrEmpty(categoryName))
    {
        query = query.Where(p => p.Category.CategoryName.ToLower().Contains(lowerCatName));
    }
    if (!string.IsNullOrEmpty(productName))
    {

        query = query.Where(p => p.ProductName.ToLower().Contains(lowerProductName));
    }
    if (!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(categoryName))
    {
        query = query.Where(p => p.Category.CategoryName.ToLower().Contains(lowerCatName)).Where(p => p.ProductName.ToLower().Contains(lowerProductName));
    }

    var products = query.ToList();
    if (!products.Any())
    {
        return Results.NotFound();
    }
    return Results.Ok(products);
});

app.MapPost("/api/products", (CornerStoreDbContext db, Product newProduct) =>
{
    if (newProduct == null)
    {
        return Results.BadRequest();
    }
    db.Products.Add(newProduct);
    db.SaveChanges();
    return Results.Created($"/api/products/{newProduct.Id}", newProduct);
});

app.MapPut("/api/products/{id}", (CornerStoreDbContext db, int id, ProductDTO updatedProduct) =>
{
    Product productToUpdate = db.Products.Where(p => p.Id == id).SingleOrDefault();
    productToUpdate.ProductName = updatedProduct.ProductName;
    productToUpdate.Price = updatedProduct.Price;
    productToUpdate.Brand = updatedProduct.Brand;
    productToUpdate.CategoryId = updatedProduct.CategoryId;

    db.SaveChanges();
    return Results.NoContent();
});

app.MapGet("/api/orders/{id}", (CornerStoreDbContext db, int id) =>
{
    //include cashier, orderProducts,then products, then categories
    var orderDto = db.Orders
    .Where(o => o.Id == id)
    .Include(o => o.Cashier)
    .Include(c => c.OrderProducts)
        .ThenInclude(op => op.Product)
        .ThenInclude(p => p.Category).Select(o => new OrderDTO
        {
            Id = o.Id,
            CashierId = o.CashierId,
            Cashier = new CashierDTO
            {
                Id = o.Cashier.Id,
                FirstName = o.Cashier.FirstName,
                LastName = o.Cashier.LastName
            },
            OrderProducts = o.OrderProducts.Select(op => new OrderProductDTO
            {
                Id = op.Id,
                ProductId = op.ProductId,
                OrderId = op.OrderId,
                Quantity = op.Quantity,
                Product = new ProductDTO
                {
                    Id = op.Product.Id,
                    ProductName = op.Product.ProductName,
                    Price = op.Product.Price,
                    Brand = op.Product.Brand,
                    CategoryId = op.Product.CategoryId,
                    Category = new CategoryDTO
                    {
                        Id = op.Product.Category.Id,
                        CategoryName = op.Product.Category.CategoryName
                    }
                }
            }).ToList(),
            PaidOnDate = o.PaidOnDate
        }).FirstOrDefault();

    if (orderDto == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(orderDto);



});

app.MapGet("/api/orders", (CornerStoreDbContext db, DateTime? orderDate) =>
{
//include what I want
//end it as Queryable
    var query = db.Orders
        .Include(o => o.Cashier)
    .Include(c => c.OrderProducts)
    .ThenInclude(op => op.Product)
    .ThenInclude(p => p.Category)
    .AsQueryable();
    
    //check if param is used
    if (orderDate.HasValue)
    {
        //seperate out date from dateTime
        var date = orderDate.Value.Date;
        //compare values
        query = query.Where(o => o.PaidOnDate.HasValue && o.PaidOnDate.Value.Date == date);
    }
    //check if orders returned anything
    var orders = query.ToList();
    if (orders.Count == 0)
    {
        return Results.NotFound();
    }
    return Results.Ok(orders);
});

app.MapDelete("/api/orders/{id}", (CornerStoreDbContext db, int id) =>
{
    Order orderToDelete = db.Orders.Where(o => o.Id == id).SingleOrDefault();
    if (orderToDelete == null)
    {
        return Results.NotFound();
    }
    db.Orders.Remove(orderToDelete);
    db.SaveChanges();
    return Results.NoContent();
});

app.MapPost("/api/orders", (CornerStoreDbContext db, OrderCreateDTO orderToCreate) =>
{
    var order = new Order
    {
        CashierId = orderToCreate.CashierId,
        PaidOnDate = orderToCreate.PaidOnDate,
        
        OrderProducts = orderToCreate.OrderProducts.Select(op => new OrderProduct
        {
            ProductId = op.ProductId,
            Quantity = op.Quantity
        }).ToList()
    };

    db.Orders.Add(order);
    db.SaveChanges();

    return Results.Created($"/api/orders/{order.Id}", order);
});

//endpoints go here

app.Run();

//don't move or change this!
public partial class Program { }