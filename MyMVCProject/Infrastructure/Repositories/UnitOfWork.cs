using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Infrastructure.Context;

namespace MyMVCProject.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RestaurantContext _context;
        public UnitOfWork(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
