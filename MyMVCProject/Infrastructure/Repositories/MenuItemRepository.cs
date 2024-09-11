using Microsoft.EntityFrameworkCore;
using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Infrastructure.Context;
using System.Linq.Expressions;

namespace MyMVCProject.Infrastructure.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly RestaurantContext _context;
        public MenuItemRepository(RestaurantContext context)
        {
            _context = context;
        }
        public async Task<MenuItem> AddAsync(MenuItem item)
        {
            await _context.MenuItems.AddAsync(item);
            return item;
        }

        public async Task<bool> ExistsAsync(string name, int menuId)
        {
            var exists = await _context.MenuItems.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.MenuId == menuId);
            return exists;
        }

        public async Task<bool> ExistsAsync(string name, int menuId, Guid id)
        {
            var exists = await _context.MenuItems.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.MenuId == menuId && x.Id != id);
            return exists;
        }

        public async Task<ICollection<MenuItem>> GetAllAsync()
        {
            return await _context.MenuItems.ToListAsync();
        }

        public async Task<MenuItem> GetAsync(Guid id)
        {
            return await _context.MenuItems.Include(x => x.Menu).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<MenuItem> GetAsync(Expression<Func<MenuItem, bool>> exp)
        {
            return await _context.MenuItems.Include(x => x.Menu).FirstOrDefaultAsync(exp);
        }

        public async Task<ICollection<MenuItem>> GetAllAsync(Expression<Func<MenuItem, bool>> exp)
        {
            return await _context.MenuItems.Where(exp).ToListAsync();
        }

        public void Remove(MenuItem item)
        {
            _context.MenuItems.Remove(item);
        }

        public MenuItem Update(MenuItem item)
        {
            _context.MenuItems.Update(item);
            return item;
        }
    }
}
