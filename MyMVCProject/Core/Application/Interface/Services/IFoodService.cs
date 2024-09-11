using MyMVCProject.Models;
using MyMVCProject.Models.FoodModel;

namespace MyMVCProject.Core.Application.Interface.Services
{
    public interface IFoodService
    {
        Task<BaseResponse> CreateFood(FoodRequest request);
        Task<BaseResponse<FoodResponse>> GetFood(int id);
        Task<BaseResponse<ICollection<FoodResponse>>> GetAllFoods();
        Task<BaseResponse> UpdateFood(int id, UpdateFoodRequest request);
        Task<BaseResponse> RemoveFood(int id);
    }
}
