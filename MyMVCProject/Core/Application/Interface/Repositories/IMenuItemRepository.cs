using MyMVCProject.Core.Domain.Entities;
using System.Linq.Expressions;

namespace MyMVCProject.Core.Application.Interface.Repositories
{
    public interface IMenuItemRepository
    {
        Task<MenuItem> AddAsync(MenuItem item);
        Task<ICollection<MenuItem>> GetAllAsync();
        Task<ICollection<MenuItem>> GetAllAsync(Expression<Func<MenuItem, bool>> exp);
        Task<MenuItem> GetAsync(Guid id);
        Task<MenuItem> GetAsync(Expression<Func<MenuItem, bool>> exp);
        Task<bool> ExistsAsync(string name, int menuId);
        Task<bool> ExistsAsync(string name, int menuId, Guid id);
        void Remove(MenuItem item);
        MenuItem Update(MenuItem item);
    }
}
