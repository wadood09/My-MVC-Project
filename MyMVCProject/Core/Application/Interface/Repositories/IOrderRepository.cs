using MyMVCProject.Core.Domain.Entities;
using System.Linq.Expressions;

namespace MyMVCProject.Core.Application.Interface.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);
        Task<ICollection<Order>> GetAllAsync();
        Task<ICollection<Order>> GetAllAsync(Expression<Func<Order, bool>> exp);
        Task<Order> GetAsync(int id);          
        Task<Order> GetAsync(Expression<Func<Order, bool>> exp);
        void Remove(Order order);
        Order Update(Order order);
    }
}
