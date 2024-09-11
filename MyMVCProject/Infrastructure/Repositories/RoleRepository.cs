using Microsoft.EntityFrameworkCore;
using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Infrastructure.Context;
using System.Linq.Expressions;

namespace MyMVCProject.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RestaurantContext _context;
        public RoleRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<Role> AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            return role;
        }

        public async Task<ICollection<Role>> GetAllAsync()
        {
            var role = await _context.Roles.ToListAsync();
            return role;
        }

        public async Task<Role> GetAsync(int id)
        {
            var role = await _context.Roles.Include(ro => ro.Users)
                .FirstOrDefaultAsync(ro => ro.Id == id);
            return role;
        }

        public async Task<Role> GetAsync(Expression<Func<Role, bool>> exp)
        {
            var role = await _context.Roles.Include(ro => ro.Users)
                .FirstOrDefaultAsync(exp);
            return role;
        }

        public async Task<bool> ExistAsync(string name)
        {
            var exists = await _context.Roles.AnyAsync(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return exists;
        }

        public void Remove(Role role)
        {
            _context.Roles.Remove(role);
        }

        public Role Update(Role role)
        {
            _context.Roles.Update(role);
            return role;
        }

        public async Task<bool> ExistAsync(string name, int id)
        {
            var exists = await _context.Roles.AnyAsync(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && x.Id != id);
            return exists;
        }
    }
}
