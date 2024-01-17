namespace CornerStore.Models.DTOs;

public class OrderDTO
{
  public int Id { get; set; }
  public int CashierId { get; set; }
  public ICollection<OrderProductDTO> OrderProducts { get; set; }
   public decimal Total
    {
        get
        {
            if (OrderProducts != null)
            {
                var orderTotal = OrderProducts.Sum(op => op.Product.Price * op.Quantity);
                return orderTotal;
            } else
            {
                return 0;
            }
        }
    } 
  public CashierDTO Cashier { get; set; }
  public DateTime ? PaidOnDate { get; set; }
}