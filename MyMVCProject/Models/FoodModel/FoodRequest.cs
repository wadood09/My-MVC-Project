namespace MyMVCProject.Models.FoodModel
{
    public class FoodRequest
    {
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
