using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.Dto.Request;

using System.ComponentModel.DataAnnotations;

public class OrderItemRequest
{
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(255, ErrorMessage = "Product name cannot exceed 255 characters.")]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0.")]
    public decimal UnitPrice { get; set; }
}
