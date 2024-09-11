using MyMVCProject.Core.Domain.Entities;
using System.Linq.Expressions;

namespace MyMVCProject.Core.Application.Interface.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> AddAsync(Customer customer);
        Task<ICollection<Customer>> GetAllAsync();
        Task<Customer> GetAsync(int id);
        Task<Customer> GetLastAsync();
        Task<Customer> GetAsync(Expression<Func<Customer, bool>> exp);
        Task<bool> ExistsAsync(string email, int id);
        void Remove(Customer customer);
        Customer Update(Customer customer);
    }
}
