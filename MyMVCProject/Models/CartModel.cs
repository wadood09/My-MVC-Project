namespace MyMVCProject.Models
{
    public class CartModel
    {
        public decimal TotalPrice { get; set; }
        public ICollection<CartItemModel> Items { get; set; } = [];
    }

    public class CartItemModel
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; } = default!;
        public int Quantity { get; set; }
        public string? Image { get; set; }
        public int RemainingQuantity { get; set; }
    }
}
