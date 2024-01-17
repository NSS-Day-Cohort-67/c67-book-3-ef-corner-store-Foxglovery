namespace CornerStore.Models.DTOs;

public class OrderCreateDTO
{
    public int CashierId { get; set; }
    public DateTime ? PaidOnDate { get; set; }
    public ICollection<OrderProductCreateDTO> OrderProducts { get; set; }
}