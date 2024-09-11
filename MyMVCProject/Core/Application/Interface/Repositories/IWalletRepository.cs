using MyMVCProject.Core.Domain.Entities;
using System.Linq.Expressions;

namespace MyMVCProject.Core.Application.Interface.Repositories
{
    public interface IWalletRepository
    {
        Task<Wallet> AddAsync(Wallet wallet);
        Task<Wallet> GetAsync(int id);
        Task<Wallet> GetAsync(Expression<Func<Wallet, bool>> exp);
        void Remove(Wallet wallet);
        Wallet Update(Wallet wallet);
    }
}
