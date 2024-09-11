using MyMVCProject.Core.Domain.Entities;

namespace MyMVCProject.Models.WalletModel
{
    public class WalletResponse
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public int UserId { get; set; }
    }
}
