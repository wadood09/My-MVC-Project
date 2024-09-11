using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Core.Application.Services;
using MyMVCProject.Models.FoodModel;
using MyMVCProject.Models.ViewModels;
using System.Security.Claims;

namespace MyMVCProject.Controllers
{
    public class FoodController : Controller
    {
        private readonly IFoodService _foodService;
        private readonly NavigationHistoryManager _historyManager;

        public FoodController(IFoodService foodService, NavigationHistoryManager historyManager)
        {
            _foodService = foodService;
            _historyManager = historyManager;
        }

        [Authorize(Roles = "Admin, Chef")]
        public IActionResult AddFood()
        {
            _historyManager.AddToHistory("Food", nameof(AddFood));

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFood(FoodRequest request)
        {
            var food = await _foodService.CreateFood(request);
            if (food.IsSuccessful)
            {
                TempData["SuccessMessage"] = food.Message;
                return RedirectToAction("getAllFood");
            }
            TempData["ErrorMessage"] = food.Message;
            return View(request);
        }

        [Authorize(Roles = "Admin, Chef")]
        public async Task<IActionResult> GetFood(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString()}
            };
            _historyManager.AddToHistory("Food", nameof(GetFood), param);

            var food = await _foodService.GetFood(id);
            if (food.IsSuccessful)
            {
                // Food details
                return View(food.Value);
            }
            // list of food
            return RedirectToAction("GetAllFood");
        }

        [Authorize(Roles = "Admin, Chef")]
        public async Task<IActionResult> GetAllFood()
        {
            _historyManager.AddToHistory("Food", nameof(GetAllFood));

            var food = await _foodService.GetAllFoods();
            // List of foods
            return View(food.Value);
            //if (food.Value.Count != 0)
            //{
                
            //}
            //// The home page
            //return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin, Chef")]
        public async Task<IActionResult> UpdateFood(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString()}
            };
            _historyManager.AddToHistory("Food", nameof(UpdateFood), param);

            var food = await _foodService.GetFood(id);
            if (food.IsSuccessful)
            {
                var updateModel = new UpdateFoodViewModel
                {
                    Name = food.Value.Name,
                    Quantity = food.Value.Quantity,
                    Price = food.Value.Price
                };
                return View(updateModel);
            }
            // The food details
            return RedirectToAction("GetFood", id);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFood(int id, [FromForm] UpdateFoodRequest request)
        {
            var food = await _foodService.UpdateFood(id, request);
            if (food.IsSuccessful)
            {
                TempData["SuccessMessage"] = food.Message;
                return RedirectToAction("GetFood", id);
            }
            var updateModel = new UpdateFoodViewModel
            {
                Name = request.Name,
                Quantity = request.Quantity,
                Price = request.Price
            };
            TempData["ErrorMessage"] = food.Message;
            return View(updateModel);
        }

        [Authorize(Roles = "Admin, Chef")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString()}
            };
            _historyManager.AddToHistory("Food", nameof(DeleteFood), param);

            var food = await _foodService.GetFood(id);
            if (food.IsSuccessful)
            {
                // confirm delete
                return View(food.Value);
            }
            // Food details
            return RedirectToAction("GetAllFood");
        }

        [HttpPost, ActionName("DeleteFood")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var food = await _foodService.RemoveFood(id);
            if (food.IsSuccessful)
            {
                TempData["SuccessMessage"] = food.Message;
                // List of foods
                return RedirectToAction("GetAllFood");
            }
            TempData["ErrorMessage"] = food.Message;
            // Food Details
            return RedirectToAction("DeleteFood", id);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
