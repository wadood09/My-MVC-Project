using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Models.FoodModel;

namespace MyMVCProject.Models.MenuModel
{
    public class MenuResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public ICollection<MenuItemResponse> Items { get; set; } = new List<MenuItemResponse>();
    }

    public class MenuItemResponse
    {
        public Guid Id { get; set; }
        public int FoodId { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
