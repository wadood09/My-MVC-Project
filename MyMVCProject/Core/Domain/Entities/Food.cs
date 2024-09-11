namespace MyMVCProject.Core.Domain.Entities
{
    public class Food : Auditables
    {
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}