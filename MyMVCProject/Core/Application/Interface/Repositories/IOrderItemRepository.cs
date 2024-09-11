using MyMVCProject.Core.Domain.Entities;
using System.Linq.Expressions;

namespace MyMVCProject.Core.Application.Interface.Repositories
{
    public interface IOrderItemRepository
    {
        Task<OrderItem> AddAsync(OrderItem item);
        Task<ICollection<OrderItem>> GetAllAsync();
        Task<OrderItem> GetAsync(Guid id);
        Task<OrderItem> GetAsync(Expression<Func<OrderItem, bool>> exp);
        void Remove(OrderItem item);
        OrderItem Update(OrderItem item);
    }
}
