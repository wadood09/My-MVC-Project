using Microsoft.EntityFrameworkCore;
using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Infrastructure.Context;
using System.Linq.Expressions;

namespace MyMVCProject.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly RestaurantContext _context;
        public WalletRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<Wallet> AddAsync(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
            return wallet;
        }

        public async Task<Wallet> GetAsync(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            return wallet;
        }

        public async Task<Wallet> GetAsync(Expression<Func<Wallet, bool>> exp)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(exp);
            return wallet;
        }

        public void Remove(Wallet wallet)
        {
            _context.Wallets.Remove(wallet);
        }

        public Wallet Update(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            return wallet;
        }
    }
}
