
namespace CornerStore.Models;

public class Order
{
    public int Id { get; set; }
    public int CashierId { get; set; }
    public ICollection<OrderProduct> OrderProducts { get; set; }
    public decimal Total => OrderProducts?.Sum(op => op.Product.Price * op.Quantity) ?? 0m;
    public Cashier Cashier { get; set; }
}