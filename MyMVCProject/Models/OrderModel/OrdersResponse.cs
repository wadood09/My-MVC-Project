using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Models.OrderModel
{
    public class OrdersResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Description { get; set; }
        public Status Status { get; set; }
    }
}
