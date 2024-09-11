using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Models.ViewModels
{
    public class UpdateOrderViewModel
    {
        public string? Description { get; set; }
        public Status? OrderStatus { get; set; }
    }
}
