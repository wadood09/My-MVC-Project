using MyMVCProject.Models.RoleModel;
using MyMVCProject.Models;

namespace MyMVCProject.Core.Application.Interface.Services
{
    public interface IRoleService
    {
        Task<BaseResponse> CreateRole(RoleRequest request);
        Task<BaseResponse<RoleResponse>> GetRole(int id);
        Task<BaseResponse<ICollection<RoleResponse>>> GetAllRole();
        Task<BaseResponse> RemoveRole(int id);
        Task<BaseResponse> UpdateRole(int id, UpdateRoleRequest request);
    }
}
