using Microsoft.EntityFrameworkCore;
using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Infrastructure.Context;
using System.Linq.Expressions;

namespace MyMVCProject.Infrastructure.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly RestaurantContext _context;
        public OrderItemRepository(RestaurantContext context)
        {
            _context = context;
        }
        public async Task<OrderItem> AddAsync(OrderItem item)
        {
            await _context.OrderItems.AddAsync(item);
            return item;
        }

        public async Task<ICollection<OrderItem>> GetAllAsync()
        {
            return await _context.OrderItems.ToListAsync();
        }

        public async Task<OrderItem> GetAsync(Guid id)
        {
            return await _context.OrderItems.FindAsync(id);
        }

        public async Task<OrderItem> GetAsync(Expression<Func<OrderItem, bool>> exp)
        {
            return await _context.OrderItems.FirstOrDefaultAsync(exp);
        }

        public void Remove(OrderItem item)
        {
            _context.OrderItems.Remove(item);
        }

        public OrderItem Update(OrderItem item)
        {
            _context.OrderItems.Update(item);
            return item;
        }
    }
}
