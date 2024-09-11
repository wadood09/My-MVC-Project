using MyMVCProject.Core.Domain.Entities;
using System.Linq.Expressions;

namespace MyMVCProject.Core.Application.Interface.Repositories
{
    public interface IFoodRepository
    {
        Task<Food> AddAsync(Food food);
        Task<ICollection<Food>> GetAllAsync();
        Task<Food> GetAsync(int id);
        Task<Food> GetAsync(Expression<Func<Food, bool>> exp);
        void Remove(Food food);
        Food Update(Food food);
        Task<bool> ExistsAsync(string name);
        Task<bool> ExistsAsync(string name, int id);
    }
}
