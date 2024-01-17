using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
namespace CornerStore.Models;

public class Product
{
    public int Id { get; set; }
    [Required]
    public string ProductName { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public string Brand { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public ICollection<OrderProduct> OrderProducts { get; set; }
}