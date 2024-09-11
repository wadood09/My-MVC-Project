using Microsoft.EntityFrameworkCore;
using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Infrastructure.Context;
using System.Linq.Expressions;

namespace MyMVCProject.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly RestaurantContext _context;
        public OrderRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<Order> AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            return order;
        }

        public async Task<ICollection<Order>> GetAllAsync()
        {
            var orders = await _context.Orders
                .Include(ord => ord.Customer).ThenInclude(cu => cu.User)
                .Include(ord => ord.Items)
                .ToListAsync();
            return orders;
        }

        public async Task<ICollection<Order>> GetAllAsync(Expression<Func<Order, bool>> exp)
        {
            var orders = await _context.Orders
                .Include(ord => ord.Customer).ThenInclude(cu => cu.User)
                .Include(ord => ord.Items)
                .Where(exp)
                .ToListAsync();
            return orders;
        }

        public async Task<Order> GetAsync(int id)
        {
            var order = await _context.Orders
                .Include(ord => ord.Customer).ThenInclude(cu => cu.User)
                .Include(ord => ord.Items)
                .FirstOrDefaultAsync(ord => ord.Id == id);
            return order;
        }

        public async Task<Order> GetAsync(Expression<Func<Order, bool>> exp)
        {
            var order = await _context.Orders
                .Include(ord => ord.Customer).ThenInclude(cu => cu.User)
                .Include(ord => ord.Items)
                .FirstOrDefaultAsync(exp);
            return order;
        }

        public void Remove(Order order)
        {
            _context.Orders.Remove(order);
        }

        public Order Update(Order order)
        {
            _context.Orders.Update(order);
            return order;
        }
    }
}
