using MyMVCProject.Core.Domain.Entities;

namespace MyMVCProject.Models.MenuModel
{
    public class UpdateMenuRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
    
    public class UpdateMenuItemRequest
    {
        public int? FoodId { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public IFormFile? Icon { get; set; }
    }
}
