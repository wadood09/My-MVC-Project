using MyMVCProject.Core.Domain.Entities;

namespace MyMVCProject.Models.MenuModel
{
    public class MenuRequest
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
    public class MenuItemRequest
    {
        public string Name { get; set; } = default!;
        public int FoodId { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public IFormFile? Icon { get; set; }
    }
}
