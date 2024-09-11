using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Models;
using MyMVCProject.Models.MenuModel;
using System.Security.Claims;

namespace MyMVCProject.Core.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileRepository _fileRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IHttpContextAccessor _httpContext;

        public MenuService(IMenuRepository menuRepository, IUnitOfWork unitOfWork, IFileRepository fileRepository, IFoodRepository foodRepository, IHttpContextAccessor httpContext, IMenuItemRepository menuItemRepository)
        {
            _menuRepository = menuRepository;
            _unitOfWork = unitOfWork;
            _fileRepository = fileRepository;
            _foodRepository = foodRepository;
            _httpContext = httpContext;
            _menuItemRepository = menuItemRepository;
        }

        public async Task<BaseResponse> CreateMenu(MenuRequest request)
        {
            var exists = await _menuRepository.ExistsAsync(request.Name);
            if (exists)
            {
                return new BaseResponse
                {
                    Message = "Menu already exists",
                    IsSuccessful = false
                };
            }

            var loginUserId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var menu = new Menu
            {
                Name = request.Name,
                CreatedBy = loginUserId,
                Description = request.Description,
            };

            await _menuRepository.AddAsync(menu);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = $"{menu.Name} menu successfully created",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> CreateMenuItem(int id, MenuItemRequest request)
        {
            var menu = await _menuRepository.GetAsync(id);
            if (menu == null)
            {
                return new BaseResponse
                {
                    Message = "Menu does not exists",
                    IsSuccessful = false
                };
            }

            var food = await _foodRepository.GetAsync(request.FoodId);
            if (food == null)
            {
                return new BaseResponse
                {
                    Message = "Food does not exists",
                    IsSuccessful = false
                };
            }

            if (food.Price != request.Price)
            {
                return new BaseResponse
                {
                    Message = "The price inputted does not equal the price of food chosen",
                    IsSuccessful = false
                };
            }

            var exists = await _menuItemRepository.ExistsAsync(request.Name, menu.Id);
            if (exists)
            {
                return new BaseResponse
                {
                    Message = $"MenuItem with name {request.Name} already exists",
                    IsSuccessful = false
                };
            }

            if (menu.Items.Any(x => x.FoodId == request.FoodId))
            {
                return new BaseResponse
                {
                    Message = $"MenuItem with that represents food chosen already exists",
                    IsSuccessful = false
                };
            }

            var menuItem = new MenuItem
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                FoodId = request.FoodId,
                MenuId = menu.Id,
                Menu = menu,
                ImageUrl = await _fileRepository.UploadAsync(request.Icon)
            };

            await _menuItemRepository.AddAsync(menuItem);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = $"{menuItem.Name} successfully added to {menu.Name} menu",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse<ICollection<MenuResponse>>> GetAllMenu()
        {
            var menus = await _menuRepository.GetAllAsync();
            return new BaseResponse<ICollection<MenuResponse>>
            {
                Message = "List of menus",
                IsSuccessful = true,
                Value = menus.Select(x => new MenuResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description ?? "This menu has no description",
                    Items = x.Items.Select(a => new MenuItemResponse
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description ?? "This item has no description",
                        Price = a.Price,
                        FoodId = a.FoodId,
                        ImageUrl = a.ImageUrl,

                    }).ToList()
                }).ToList()
            };
        }

        public async Task<BaseResponse<MenuResponse>> GetMenu(int id)
        {
            var menu = await _menuRepository.GetAsync(id);
            if (menu == null)
            {
                return new BaseResponse<MenuResponse>
                {
                    Message = "Menu does not exists",
                    IsSuccessful = false
                };
            }
            return new BaseResponse<MenuResponse>
            {
                Message = "Menu successfully found",
                IsSuccessful = true,
                Value = new MenuResponse
                {
                    Id = id,
                    Name = menu.Name,
                    Description = menu.Description,
                    Items = menu.Items.Select(a => new MenuItemResponse
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description ?? "This menu has no description",
                        Price = a.Price,
                        FoodId = a.FoodId,
                        ImageUrl = a.ImageUrl
                    }).ToList()
                }
            };
        }

        public async Task<BaseResponse<MenuItemResponse>> GetMenuItem(Guid id)
        {
            var menuItem = await _menuItemRepository.GetAsync(id);
            if (menuItem == null)
            {
                return new BaseResponse<MenuItemResponse>
                {
                    Message = "MenuItem does not exists",
                    IsSuccessful = false
                };
            }
            return new BaseResponse<MenuItemResponse>
            {
                Message = "MenuItem successfully found",
                IsSuccessful = true,
                Value = new MenuItemResponse
                {
                    Id = menuItem.Id,
                    Description = menuItem.Description,
                    Name = menuItem.Name,
                    FoodId = menuItem.FoodId,
                    Price = menuItem.Price,
                    ImageUrl = menuItem.ImageUrl
                }
            };
        }

        public async Task<BaseResponse> UpdateMenu(int id, UpdateMenuRequest request)
        {
            var menu = await _menuRepository.GetAsync(id);
            if (menu == null)
            {
                return new BaseResponse
                {
                    Message = "Menu does not exists",
                    IsSuccessful = false
                };
            }

            var exists = await _menuRepository.ExistsAsync(request.Name, id);
            if (exists)
            {
                return new BaseResponse
                {
                    Message = $"Menu with name '{request.Name}' already exists",
                    IsSuccessful = false
                };
            }

            var loginUserId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            menu.DateModified = DateTime.Now;
            menu.ModifiedBy = loginUserId;
            menu.Name = request.Name ?? menu.Name;
            menu.Description = request.Description;

            _menuRepository.Update(menu);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Menu updated successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> UpdateMenuItem(Guid id, UpdateMenuItemRequest request)
        {
            var menuItem = await _menuItemRepository.GetAsync(id);
            if (menuItem == null)
            {
                return new BaseResponse
                {
                    Message = "MenuItem does not exists",
                    IsSuccessful = false
                };
            }
                
            var food = await _foodRepository.GetAsync(menuItem.FoodId);
            if (request.FoodId.HasValue)
            {
                food = await _foodRepository.GetAsync(request.FoodId.Value);
                if (food == null)
                {
                    return new BaseResponse
                    {
                        Message = "Food does not exists",
                        IsSuccessful = false
                    };
                }

                if (request.Price.HasValue)
                {
                    if (food.Price != request.Price.Value)
                    {
                        return new BaseResponse
                        {
                            Message = "The price inputted does not equal the price of food chosen",
                            IsSuccessful = false
                        };
                    }
                }

                if (food.Price != menuItem.Price)
                {
                    return new BaseResponse
                    {
                        Message = "The former price of menu item does not equal the price of food chosen",
                        IsSuccessful = false
                    };
                }
            }

            if (request.Price.HasValue)
            {
                if (food.Price != request.Price.Value)
                {
                    return new BaseResponse
                    {
                        Message = "The new price of menu item does not equal the price of food it represents",
                        IsSuccessful = false
                    };
                }
            }

            if (request.Name != null)
            {
                var exists = await _menuItemRepository.ExistsAsync(request.Name, menuItem.MenuId, id);
                if (exists)
                {
                    return new BaseResponse
                    {
                        Message = $"MenuItem with name {request.Name} already exists",
                        IsSuccessful = false
                    };
                }
            }

            menuItem.Name = request.Name ?? menuItem.Name;
            menuItem.Description = request.Description ?? menuItem.Description;
            menuItem.Price = request.Price ?? menuItem.Price;
            menuItem.FoodId = request.FoodId ?? menuItem.FoodId;
            menuItem.ImageUrl = await _fileRepository.UploadAsync(request.Icon) ?? menuItem.ImageUrl;


            _menuItemRepository.Update(menuItem);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Menu item updated successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> RemoveMenu(int id)
        {
            var menu = await _menuRepository.GetAsync(id);
            if (menu == null)
            {
                return new BaseResponse
                {
                    Message = "Menu does not exists",
                    IsSuccessful = false
                };
            }

            _menuRepository.Remove(menu);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Menu successfully deleted",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> RemoveMenuItem(Guid id)
        {
            var menuItem = await _menuItemRepository.GetAsync(id);
            if (menuItem == null)
            {
                return new BaseResponse
                {
                    Message = "MenuItem does not exists",
                    IsSuccessful = false
                };
            }

            _menuItemRepository.Update(menuItem);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Menu item deleted successfully",
                IsSuccessful = true
            };

        }
    }
}
