namespace MyMVCProject.Models.ViewModels
{
    public class UpdateMenuItemViewModel
    {
        public int? FoodId { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public IFormFile? Icon { get; set; }
    }
}
