using Microsoft.EntityFrameworkCore;
using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Infrastructure.Context;
using System.Linq.Expressions;

namespace MyMVCProject.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly RestaurantContext _context;
        public CustomerRepository(RestaurantContext context)
        {
            _context = context;
        }
        public async Task<Customer> AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            return customer;
        }

        public async Task<Customer> GetAsync(int id)
        {
            return await _context.Customers.Include(x => x.Wallet).Include(cu => cu.User).FirstOrDefaultAsync(cu => cu.Id == id);
        }
        
        public async Task<Customer> GetLastAsync()
        {
            return await _context.Customers.Include(cu => cu.User).OrderBy(x => x.DateCreated).LastAsync();
        }

        public async Task<ICollection<Customer>> GetAllAsync()
        {
            return await _context.Customers.Include(cu => cu.User).ToListAsync();
        }

        public async Task<Customer> GetAsync(Expression<Func<Customer, bool>> exp)
        {
            return await _context.Customers.Include(x => x.Wallet).Include(cu => cu.User).Include(x => x.Orders).FirstOrDefaultAsync(exp);
        }

        public void Remove(Customer customer)
        {
            _context.Customers.Remove(customer);
        }

        public Customer Update(Customer customer)
        {
            _context.Customers.Update(customer);
            return customer;
        }

        public async Task<bool> ExistsAsync(string email, int id)
        {
            var exists = await _context.Customers.AnyAsync(cus => cus.User.Email == email && cus.Id != id);
            return exists;
        }
    }
}
