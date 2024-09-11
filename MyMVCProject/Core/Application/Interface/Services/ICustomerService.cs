using MyMVCProject.Models;
using MyMVCProject.Models.CustomerModel;

namespace MyMVCProject.Core.Application.Interface.Services
{
    public interface ICustomerService
    {
        Task<BaseResponse> CreateCustomer(CustomerRequest request);
        Task<BaseResponse<CustomerResponse>> GetCustomer(int id);
        Task<BaseResponse<CustomerResponse>> GetCustomerByUserId(int id);
        Task<BaseResponse<ICollection<CustomersResponse>>> GetAllCustomer();
        Task<BaseResponse> UpdateCustomer(int id, UpdateCustomerRequest request);
        Task<BaseResponse> RemoveCustomer(int id);
        Task<int> GetLoginCustomerId();
    }
}
