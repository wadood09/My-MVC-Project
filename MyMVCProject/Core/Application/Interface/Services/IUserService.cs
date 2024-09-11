using MyMVCProject.Models.UserModel;
using MyMVCProject.Models;

namespace MyMVCProject.Core.Application.Interface.Services
{
    public interface IUserService
    {
        Task<BaseResponse> CreateUser(UserRequest request);
        Task<BaseResponse<UserResponse>> GetUser(int id);
        Task<BaseResponse<ICollection<UsersResponse>>> GetAllUsers();
        Task<BaseResponse> RemoveUser(int id);
        Task<BaseResponse> UpdateUser(int id, UpdateUserRequest request);
        Task<BaseResponse<UserResponse>> Login(LoginRequestModel model);
    }
}