using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Models.OrderModel
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Description { get; set; }
        public Status OrderStatus { get; set; }
        public ICollection<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
    }

    public class OrderItemResponse
    {
        public Guid Id { get; set; }
        public string FoodName { get; set; } = default!;
        public int FoodId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Units { get; set; }
        public string? Image { get; set; }
    }
}
