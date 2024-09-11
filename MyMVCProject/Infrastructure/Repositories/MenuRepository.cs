using Microsoft.EntityFrameworkCore;
using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Infrastructure.Context;
using System.Linq.Expressions;

namespace MyMVCProject.Infrastructure.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly RestaurantContext _context;
        public MenuRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<Menu> AddAsync(Menu menu)
        {
            await _context.Menus.AddAsync(menu);
            return menu;
        }

        public async Task<bool> ExistsAsync(string name)
        {
            var exist = await _context.Menus.AnyAsync(m => m.Name == name);
            return exist;
        }

        public async Task<bool> ExistsAsync(string name, int id)
        {
            var exist = await _context.Menus.AnyAsync(m => m.Name == name && m.Id != id);
            return exist;
        }

        public async Task<ICollection<Menu>> GetAllAsync()
        {
            var menus = await _context.Menus.Include(menu => menu.Items).ToListAsync();
            return menus;
        }

        public async Task<Menu> GetAsync(int id)
        {
            var menu = await _context.Menus.Include(x => x.Items).FirstOrDefaultAsync(m => m.Id == id);
            return menu;
        }

        public async Task<Menu> GetAsync(Expression<Func<Menu, bool>> exp)
        {
            var menu = await _context.Menus.Include(menu => menu.Items).FirstOrDefaultAsync(exp);
            return menu;
        }

        public void Remove(Menu menu)
        {
            _context.Menus.Remove(menu);
        }

        public Menu Update(Menu menu)
        {
            _context.Menus.Update(menu);
            return menu;
        }
    }
}
