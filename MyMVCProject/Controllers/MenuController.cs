using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Core.Application.Services;
using MyMVCProject.Models.MenuModel;
using MyMVCProject.Models.ViewModels;
using System.Security.Claims;

namespace MyMVCProject.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IFoodService _foodService;
        private readonly NavigationHistoryManager _historyManager;

        public MenuController(IMenuService menuService, IFoodService foodService, NavigationHistoryManager historyManager)
        {
            _menuService = menuService;
            _foodService = foodService;
            _historyManager = historyManager;
        }

        [Authorize(Roles = "Admin, Chef")]
        public IActionResult AddMenu()
        {
            _historyManager.AddToHistory("Menu", nameof(AddMenu));

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMenu(MenuRequest request)
        {
            var menu = await _menuService.CreateMenu(request);
            if (menu.IsSuccessful)
            {
                // List of menu
                TempData["SuccessMessage"] = menu.Message;
                return RedirectToAction("GetAllMenu");
            }
            TempData["ErrorMessage"] = menu.Message;
            return View(request);
        }

        [Authorize(Roles = "Admin, Chef")]
        public async Task<IActionResult> AddMenuItem()
        {
            _historyManager.AddToHistory("Menu", nameof(AddMenuItem));

            var foods = await _foodService.GetAllFoods();
            ViewBag.foods = new SelectList(foods.Value, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMenuItem(int id, [FromForm] MenuItemRequest request)
        {
            var menu = await _menuService.CreateMenuItem(id, request);
            if (menu.IsSuccessful)
            {
                TempData["SuccessMessage"] = menu.Message;
                return RedirectToAction("GetMenu", new { id });
            }
            var foods = await _foodService.GetAllFoods();
            ViewBag.foods = new SelectList(foods.Value, "Id", "Name");
            TempData["ErrorMessage"] = menu.Message;
            return View(request);
        }

        [Authorize(Roles = "Admin, Chef")]
        public async Task<IActionResult> GetMenu(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString()}
            };
            _historyManager.AddToHistory("Menu", nameof(GetMenu), param);

            var menu = await _menuService.GetMenu(id);
            if (menu.IsSuccessful)
            {
                // Menu details
                return View(menu.Value);
            }
            // List of Menu
            return RedirectToAction("GetAllMenu");
        }

        [Authorize(Roles = "Admin, Chef, Customer")]
        public async Task<IActionResult> GetAllMenu()
        {
            _historyManager.AddToHistory("Menu", nameof(GetAllMenu));

            var menu = await _menuService.GetAllMenu();
            return View(menu.Value);
            //if (menu.Value.Count != 0)
            //{

            //}
            //return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin, Chef")]
        public async Task<IActionResult> UpdateMenu(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString()}
            };
            _historyManager.AddToHistory("Menu", nameof(UpdateMenu), param);

            var menu = await _menuService.GetMenu(id);
            if (menu.IsSuccessful)
            {
                var model = new UpdateMenuViewModel
                {
                    Name = menu.Value.Name,
                    Description = menu.Value.Description
                };
                return View(model);
            }
            // Menu details
            return RedirectToAction("GetMenu", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMenu(int id, [FromForm] UpdateMenuRequest request)
        {
            var menu = await _menuService.UpdateMenu(id, request);
            if (menu.IsSuccessful)
            {
                // MEnu details
                TempData["SuccessMessage"] = menu.Message;
                return RedirectToAction("GoBack", "User");
            }
            var model = new UpdateMenuViewModel
            {
                Name = request.Name,
                Description = request.Description
            };
            TempData["ErrorMessage"] = menu.Message;
            return View(model);
        }

        [Authorize(Roles = "Admin, Chef")]
        public async Task<IActionResult> UpdateMenuItem(int menuId, Guid id)
        {
            var param = new Dictionary<string, string>
            {
                {"menuId", menuId.ToString()},
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("Menu", nameof(UpdateMenuItem), param);

            var menu = await _menuService.GetMenuItem(id);
            if (menu.IsSuccessful)
            {
                var foods = await _foodService.GetAllFoods();
                ViewBag.foods = new SelectList(foods.Value, "Id", "Name", foods.Value.First(x => x.Id == menu.Value.FoodId));
                var model = new UpdateMenuItemViewModel
                {
                    Name = menu.Value.Name,
                    Description = menu.Value.Description,
                    FoodId = menu.Value.FoodId,
                    Price = menu.Value.Price
                };
                return View(model);
            }
            // Menu details
            return RedirectToAction("GetMenu", new { id = menuId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMenuItem(int menuId, Guid id, [FromForm] UpdateMenuItemRequest request)
        {
            var menu = await _menuService.UpdateMenuItem(id, request);
            if (menu.IsSuccessful)
            {
                // Menu details
                TempData["SuccessMessage"] = menu.Message;
                return RedirectToAction("GetMenu", new { id = menuId });
            }
            var foods = await _foodService.GetAllFoods();
            ViewBag.foods = new SelectList(foods.Value, "Id", "Name");
            var model = new UpdateMenuItemViewModel
            {
                Name = request.Name,
                Description = request.Description,
                FoodId = request.FoodId,
                Price = request.Price,
                Icon = request.Icon
            };
            TempData["ErrorMessage"] = menu.Message;
            return View(model);
        }

        [Authorize(Roles = "Admin, Chef")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("Menu", nameof(DeleteMenu), param);

            var menu = await _menuService.GetMenu(id);
            if (menu.IsSuccessful)
            {
                // Confirm page
                return View(menu.Value);
            }
            // Menu details
            return RedirectToAction("GetMenu", new { id });
        }

        [HttpPost, ActionName("DeleteMenu")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menu = await _menuService.RemoveMenu(id);
            if (menu.IsSuccessful)
            {
                // List of menu
                return RedirectToAction("GetAllMenu");
            }
            // Menu details
            return RedirectToAction("GetMenu", new { id });
        }

        [Authorize(Roles = "Admin, Chef")]
        public async Task<IActionResult> DeleteMenuItem(int menuId, Guid id)
        {
            var param = new Dictionary<string, string>
            {
                {"menuId", menuId.ToString()},
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("Menu", nameof(DeleteMenuItem), param);

            var menu = await _menuService.GetMenuItem(id);
            if (menu.IsSuccessful)
            {
                // Confirm page
                return View(menu.Value);
            }
            // Menu details
            return RedirectToAction("GetMenu", new { id });
        }

        [HttpPost, ActionName("DeleteMenuItem")]
        public async Task<IActionResult> ItemDeleteConfirmed(int menuId, Guid id)
        {
            var menu = await _menuService.RemoveMenuItem(id);
            if (menu.IsSuccessful)
            {
                // Menu details
                TempData["SuccessMessage"] = menu.Message;
                return RedirectToAction("GetMenu", new { id = menuId });
            }
            // Menu details
            TempData["ErrorMessage"] = menu.Message;
            return RedirectToAction("GetMenu", new { id = menuId });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
