using Microsoft.EntityFrameworkCore;
using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Infrastructure.Context;
using System.Linq.Expressions;

namespace MyMVCProject.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RestaurantContext _context;
        public UserRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            user = await _context.Users.OrderBy(x => x.DateCreated).LastAsync();
            return user;
        }

        public async Task<bool> ExistsAsync(string email)
        {
            var exists = await _context.Users.AnyAsync(x => x.Email == email);
            return exists;
        }
        
        public async Task<bool> ExistsAsync(string email, int id)
        {
            var exists = await _context.Users.AnyAsync(x => x.Email == email && x.Id != id);
            return exists;
        }

        public async Task<User> GetAsync(int id)
        {
            var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == id);
            return user;
        }

        public async Task<User> GetAsync(Expression<Func<User, bool>> exp)
        {
            var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(exp);
            return user;
        }

        public async Task<ICollection<User>> GetAllAsync()
        {
            var user = await _context.Users.Include(x => x.Role).ToListAsync();
            return user.SkipWhile(x => x.Role.Name == "Customer" || x.Role.Name == "CUSTOMER").ToList();
        }

        public void Remove(User user)
        {
            _context.Users.Remove(user);
        }

        public User Update(User user)
        {
            _context.Users.Update(user);
            return user;
        }
    }
}
