using MyMVCProject.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Core.Domain.Entities
{
    public class Order : Auditables
    {
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string? Description { get; set; }
        public Status OrderStatus { get; set; } = Status.Pending;
    }

    public class OrderItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FoodName { get; set; } = default!;
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int FoodId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Units { get; set; }
    }
}
