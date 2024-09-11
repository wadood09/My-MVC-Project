using MyMVCProject.Core.Domain.Entities;
using System.Linq.Expressions;

namespace MyMVCProject.Core.Application.Interface.Repositories
{
    public interface IRoleRepository
    {
        Task<Role> AddAsync(Role role);
        Task<Role> GetAsync(int id);
        Task<Role> GetAsync(Expression<Func<Role, bool>> exp);
        Task<ICollection<Role>> GetAllAsync();
        void Remove(Role role);
        Task<bool> ExistAsync(string name);
        Task<bool> ExistAsync(string name, int id);
        Role Update(Role role);
    }
}
