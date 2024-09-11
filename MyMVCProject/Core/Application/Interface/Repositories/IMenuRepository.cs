using MyMVCProject.Core.Domain.Entities;
using System.Linq.Expressions;

namespace MyMVCProject.Core.Application.Interface.Repositories
{
    public interface IMenuRepository
    {
        Task<Menu> AddAsync(Menu menu);
        Task<ICollection<Menu>> GetAllAsync();
        Task<Menu> GetAsync(int id);
        Task<Menu> GetAsync(Expression<Func<Menu, bool>> exp);
        void Remove(Menu menu);
        Menu Update(Menu menu);
        Task<bool> ExistsAsync(string name);
        Task<bool> ExistsAsync(string name, int id);
    }
}
