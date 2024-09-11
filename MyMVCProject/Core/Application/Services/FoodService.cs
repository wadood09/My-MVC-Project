using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Models;
using MyMVCProject.Models.FoodModel;
using System.Security.Claims;

namespace MyMVCProject.Core.Application.Services
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUnitOfWork _unitOfWork;

        public FoodService(IFoodRepository foodRepository, IHttpContextAccessor httpContext, IUnitOfWork unitOfWork, IMenuItemRepository menuItemRepository)
        {
            _foodRepository = foodRepository;
            _httpContext = httpContext;
            _unitOfWork = unitOfWork;
            _menuItemRepository = menuItemRepository;
        }

        public async Task<BaseResponse> CreateFood(FoodRequest request)
        {
            var exists = await _foodRepository.ExistsAsync(request.Name);
            if (exists)
            {
                return new BaseResponse
                {
                    Message = $"Food with name '{request.Name}' already exists",
                    IsSuccessful = false
                };
            }

            var loginUserId = _httpContext.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var food = new Food
            {
                Quantity = request.Quantity,
                Name = request.Name,
                Price = request.Price,
                CreatedBy = loginUserId
            };

            await _foodRepository.AddAsync(food);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = $"{food.Name} added successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse<ICollection<FoodResponse>>> GetAllFoods()
        {
            var foods = await _foodRepository.GetAllAsync();
            return new BaseResponse<ICollection<FoodResponse>>
            {
                Message = "List of foods",
                IsSuccessful = true,
                Value = foods.Select(x => new FoodResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity
                }).ToList()
            };
        }

        public async Task<BaseResponse<FoodResponse>> GetFood(int id)
        {
            var food = await _foodRepository.GetAsync(id);
            if (food == null)
            {
                return new BaseResponse<FoodResponse>
                {
                    Message = "Food does not exists",
                    IsSuccessful = false,
                };
            }

            return new BaseResponse<FoodResponse>
            {
                Message = "Food successfully found",
                IsSuccessful = true,
                Value = new FoodResponse
                {
                    Id = food.Id,
                    Name = food.Name,
                    Price = food.Price,
                    Quantity = food.Quantity
                }
            };
        }

        public async Task<BaseResponse> RemoveFood(int id)
        {
            var food = await _foodRepository.GetAsync(id);
            if (food == null)
            {
                return new BaseResponse
                {
                    Message = "Food not found",
                    IsSuccessful = false
                };
            }

            _foodRepository.Remove(food);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = $"{food.Name} removed successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> UpdateFood(int id, UpdateFoodRequest request)
        {
            var food = await _foodRepository.GetAsync(id);
            if (food == null)
            {
                return new BaseResponse
                {
                    Message = "Food does not exists",
                    IsSuccessful = false
                };
            }

            var exists = await _foodRepository.ExistsAsync(request.Name, id);
            if (exists)
            {
                return new BaseResponse
                {
                    Message = $"Food with name '{request.Name}' already exists",
                    IsSuccessful = false
                };
            }

            if(request.Quantity.HasValue)
            {
                if(request.Quantity.Value < food.Quantity)
                {
                    return new BaseResponse
                    {
                        Message = $"The new quantity of food must not be less than the former amount which is {food.Quantity}",
                        IsSuccessful = false
                    };
                }
            }

            var loginUserId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            food.DateModified = DateTime.Now;
            food.ModifiedBy = loginUserId;
            food.Name = request.Name ?? food.Name;
            food.Price = request.Price ?? food.Price;
            food.Quantity = request.Quantity ?? food.Quantity;

            var menuItems = await _menuItemRepository.GetAllAsync(x => x.FoodId == food.Id);
            menuItems.ToList().ForEach(x =>
            {
                x.Price = food.Price;
                _menuItemRepository.Update(x);
            });
            _foodRepository.Update(food);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = $"{food.Name} successfully updated",
                IsSuccessful = true
            };
        }
    }
}
