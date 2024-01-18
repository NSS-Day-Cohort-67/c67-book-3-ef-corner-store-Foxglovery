
namespace CornerStore.Models;

public class Order
{
    public int Id { get; set; }
    public int CashierId { get; set; }
    public ICollection<OrderProduct> OrderProducts { get; set; }
    
    public decimal? Total
    {
        //Look at all these null checks(?)...it needed all of these to make it work. REMEMBER THAT
        get
        {
            if (OrderProducts != null)
            {
                var orderTotal = OrderProducts?.Sum(op => op?.Product?.Price * op.Quantity);
                return orderTotal;
            } else
            {
                return 0m;
            }
        }
    } 
   
    public Cashier Cashier { get; set; }
    public DateTime ? PaidOnDate { get; set; }
}