using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Models.OrderModel
{
    public class UpdateOrderRequest
    {
        public string? Description { get; set; }
        public Status? OrderStatus { get; set; }
    }
    
    public class UpdateOrderItemRequest
    {
        public int? FoodId { get; set; }
        public int? Quantity { get; set; }
    }
}
