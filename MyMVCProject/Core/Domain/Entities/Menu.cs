namespace MyMVCProject.Core.Domain.Entities
{
    public class Menu : Auditables
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public ICollection<MenuItem> Items { get; set; } = new List<MenuItem>();
    }

    public class MenuItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int MenuId { get; set; }
        public Menu Menu { get; set; }
        public int FoodId { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
