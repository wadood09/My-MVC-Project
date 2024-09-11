namespace MyMVCProject.Core.Domain.Entities
{
    public class Customer : Auditables
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public Wallet Wallet { get; set; }
        public int WalletId { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}