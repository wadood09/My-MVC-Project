namespace MyMVCProject.Models.FoodModel
{
    public class FoodResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
