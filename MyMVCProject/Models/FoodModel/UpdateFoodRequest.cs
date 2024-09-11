using MyMVCProject.Core.Domain.Entities;

namespace MyMVCProject.Models.FoodModel
{
    public class UpdateFoodRequest
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
    }
}
