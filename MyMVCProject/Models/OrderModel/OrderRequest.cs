using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Models.OrderModel
{
    public class OrderRequest
    {
        public ICollection<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
        public int CustomerId { get; set; }
        public string? Description { get; set; }
    }

    public class OrderItemRequest
    {
        public int FoodId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
