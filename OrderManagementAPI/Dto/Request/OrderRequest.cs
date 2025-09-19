using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.Dto.Request;

public class OrderRequest
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    [MinLength(1)]
    public List<OrderItemRequest> Items { get; set; } = [];
}