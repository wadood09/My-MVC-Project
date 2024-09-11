using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Models.OrderModel
{
    public class CustomersOrderResponse
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Description { get; set; }
        public Status Status { get; set; }
    }
}
