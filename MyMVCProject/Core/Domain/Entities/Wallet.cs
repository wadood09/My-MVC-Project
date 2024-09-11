namespace MyMVCProject.Core.Domain.Entities
{
    public class Wallet : Auditables
    {
        public decimal Balance { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }  
    }
}
