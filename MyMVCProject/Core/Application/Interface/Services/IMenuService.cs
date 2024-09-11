using MyMVCProject.Models;
using MyMVCProject.Models.MenuModel;

namespace MyMVCProject.Core.Application.Interface.Services
{
    public interface IMenuService
    {
        Task<BaseResponse> CreateMenu(MenuRequest request);
        Task<BaseResponse> CreateMenuItem(int id, MenuItemRequest request);
        Task<BaseResponse<MenuItemResponse>> GetMenuItem(Guid id);
        Task<BaseResponse<ICollection<MenuResponse>>> GetAllMenu();
        Task<BaseResponse<MenuResponse>> GetMenu(int id);
        Task<BaseResponse> UpdateMenu(int id, UpdateMenuRequest request);
        Task<BaseResponse> UpdateMenuItem(Guid id, UpdateMenuItemRequest request);
        Task<BaseResponse> RemoveMenu(int id);
        Task<BaseResponse> RemoveMenuItem(Guid id);
    }
}
