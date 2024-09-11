using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Core.Application.Services;
using MyMVCProject.Models.OrderModel;
using MyMVCProject.Models.ViewModels;
using System.Security.Claims;

namespace MyMVCProject.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IFoodService _foodService;
        private readonly NavigationHistoryManager _historyManager;

        public OrderController(IOrderService orderService, ICustomerService customerService, IFoodService foodService, NavigationHistoryManager historyManager)
        {
            _orderService = orderService;
            _customerService = customerService;
            _foodService = foodService;
            _historyManager = historyManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Customer")]
        public IActionResult AddOrder()
        {
            _historyManager.AddToHistory("Order", nameof(AddOrder));

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(OrderRequest request)
        {
            var order = await _orderService.CreateOrder(request);
            if (order.IsSuccessful)
            {
                TempData["SuccessMessage"] = order.Message;
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = order.Message;
            return View(request);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddOrderItem(int foodId = 0)
        {
            var param = new Dictionary<string, string>
            {
                {"foodId", foodId.ToString() }
            };
            _historyManager.AddToHistory("Order", nameof(AddOrderItem), param);

            if (foodId != 0)
            {
                TempData["Cart"] = "cart";
                TempData["FoodId"] = foodId;
            }
            else
            {
                var food = await _foodService.GetAllFoods();
                ViewBag.foods = new SelectList(food.Value, "Id", "Name");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddOrderItem(OrderItemRequest request)
        {
            if (request.FoodId == 0)
            {
                request.FoodId = int.Parse(TempData["FoodId"].ToString());
                TempData["Cart"] = "cart";
            }
            else
            {
                var food = await _foodService.GetAllFoods();
                ViewBag.foods = new SelectList(food.Value, "Id", "Name");
            }
            var order = await _orderService.CreateOrderItem(request);
            if (order.IsSuccessful)
            {
                TempData["SuccessMessage"] = order.Message;
                return RedirectToAction("GetAllMenu", "Menu");
            }
            TempData["ErrorMessage"] = order.Message;
            return RedirectToAction("ReView", "User");
        }

        [Authorize(Roles = "Admin, Customer, Chef")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("Order", nameof(GetOrder), param);

            var order = await _orderService.GetOrder(id);
            return View(order.Value);
            //if (order.IsSuccessful)
            //{
            //    return View(order.Value);
            //}
            //var role = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Role).Value;
            //if (role == "Customer")
            //{
            //    var loginUserId = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            //    var customer = await _customerService.GetCustomerByUserId(int.Parse(loginUserId));
            //    return RedirectToAction("GetAllOrderByCustomer", customer.Value.Id);
            //}
            //return RedirectToAction("GetAllOrder");
        }

        [Authorize(Roles = "Admin, Customer, Chef")]
        public async Task<IActionResult> GetAllOrderByCustomer(int customerId)
        {
            var param = new Dictionary<string, string>
            {
                {"customerId", customerId.ToString() }
            };
            _historyManager.AddToHistory("Order", nameof(GetAllOrderByCustomer), param);

            var order = await _orderService.GetAllOrdersByCustomer(customerId);
            if (order.Value != null)
            {
                return View(order.Value);
            }
            TempData["OrderErrorMessage"] = "No order has yet to be made by customer";
            return RedirectToAction("GoBack", "User", new { index = 4 });
        }

        [Authorize(Roles = "Admin, Customer, Chef")]
        public async Task<IActionResult> GetAllOrder()
        {
            _historyManager.AddToHistory("Order", nameof(GetAllOrder));

            var order = await _orderService.GetAllOrders();
            return View(order.Value);

            //if (order.Value.Count != 0)
            //{
            //}
            //TempData["ErrorMessage"] = "No order has yet to be made";
            //return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, Customer, Chef")]
        public async Task<IActionResult> UpdateOrder(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("Order", nameof(UpdateOrder), param);

            var order = await _orderService.GetOrder(id);
            if (order.IsSuccessful)
            {
                var model = new UpdateOrderViewModel
                {
                    Description = order.Value.Description,
                    OrderStatus = order.Value.OrderStatus
                };
                return View(model);
            }
            return RedirectToAction("GetOrder", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderRequest request)
        {
            var order = await _orderService.UpdateOrder(id, request);
            if (order.IsSuccessful)
            {
                return RedirectToAction("GoBack", "User");
            }

            var model = new UpdateOrderViewModel
            {
                Description = request.Description,
                OrderStatus = request.OrderStatus
            };
            TempData["ErrorMessage"] = order.Message;
            return View(request);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateOrderItem(Guid id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("Order", nameof(UpdateOrderItem), param);

            var order = await _orderService.GetOrderItem(id);
            if (order.IsSuccessful)
            {
                var model = new UpdateOrderItemViewModel
                {
                    FoodId = order.Value.FoodId,
                    Quantity = order.Value.Units
                };
                return View(model);
            }
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderItem(Guid id, UpdateOrderItemRequest request)
        {
            var order = await _orderService.UpdateOrderItem(id, request);
            if (order.IsSuccessful)
            {
                TempData["SuccessMessage"] = order.Message;
                return RedirectToAction("Cart");
            }
            var model = new UpdateOrderItemViewModel
            {
                FoodId = request.FoodId,
                Quantity = request.Quantity
            };
            TempData["ErrorMessage"] = order.Message;
            return View();
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveOrder(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("Order", nameof(RemoveOrder), param);

            var order = await _orderService.GetOrder(id);
            if (order.IsSuccessful)
            {
                return View(order.Value);
            }
            return RedirectToAction("GetOrder", new { id });
        }

        [HttpPost, ActionName("RemoveOrder")]
        public async Task<IActionResult> RemoveOrderConfirmed(int id)
        {
            var menu = await _orderService.DeleteOrder(id);
            if (menu.IsSuccessful)
            {
                return RedirectToAction("GoBack", "User");
            }
            TempData["ErrorMessage"] = menu.Message;
            return RedirectToAction("RemoveOrder", new { id });
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveOrderItem(Guid id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("Order", nameof(RemoveOrderItem), param);

            var order = await _orderService.GetOrderItem(id);
            if (order.IsSuccessful)
            {
                return View(order.Value);
            }
            return RedirectToAction("GetCart");
        }

        [HttpPost, ActionName("RemoveOrderItem")]
        public async Task<IActionResult> RemoveOrderItemConfirmed(Guid id)
        {
            var order = await _orderService.DeleteOrderItem(id);
            if (order.IsSuccessful)
            {
                TempData["SuccessMessage"] = order.Message;
                return RedirectToAction("GetCart");
            }
            TempData["ErrorMessage"] = order.Message;
            return RedirectToAction("ReView", "User");
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Cart()
        {
            _historyManager.AddToHistory("Order", nameof(Cart));

            var order = await _orderService.GetCurrentOrder();
            if (order.IsSuccessful)
            {
                return View(order.Value);
            }
            TempData["ErrorMessage"] = "Cart is empty";
            return View(order.Value);
        }

        [Authorize(Roles = "Admin, Customer, Chef")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("Order", nameof(CancelOrder), param);

            var order = await _orderService.GetOrder(id);
            if (order.IsSuccessful)
            {
                return View(order.Value);
            }
            TempData["ErrorMessage"] = order.Message;
            return View("GetAllOrderByCustomer");
        }

        [HttpPost, ActionName("CancelOrder")]
        public async Task<IActionResult> CancelOrderConfirmed(int id)
        {
            var order = await _orderService.CancelOrder(id);
            if (order.IsSuccessful)
            {
                TempData["SuccessMessage"] = order.Message;
                return RedirectToAction("GoBack", "User");
            }
            TempData["ErrorMessage"] = order.Message;
            return RedirectToAction("CancelOrder", new { id });
        }

        [Authorize(Roles = "Admin, Chef")]
        public async Task<IActionResult> VerifyOrder(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("Order", nameof(VerifyOrder), param);

            var order = await _orderService.GetOrder(id);
            if (order.IsSuccessful)
            {
                return View(order.Value);
            }
            TempData["ErrorMessage"] = order.Message;
            return View("GetAllOrderByCustomer");
        }

        [HttpPost, ActionName("VerifyOrder")]
        public async Task<IActionResult> VerifyOrderConfirmed(int id)
        {
            var order = await _orderService.VerifyOrder(id);
            if (order.IsSuccessful)
            {
                TempData["SuccessMessage"] = order.Message;
                return RedirectToAction("GoBack", "User");
            }
            TempData["ErrorMessage"] = order.Message;
            return RedirectToAction("VerifyOrder", new { id });
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MakeOrder()
        {
            var order = await _orderService.MakeOrder();
            if (order.IsSuccessful)
            {
                TempData["SuccessMessage"] = order.Message;
                return RedirectToAction("GetAllMenu", "Menu");
            }
            TempData["ErrorMessage"] = order.Message;
            return RedirectToAction("GetCart");
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCart()
        {
            _historyManager.AddToHistory("Order", nameof(GetCart));

            var cart = await _orderService.GetCart();
            return View(cart.Value);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> ChangeOrderItemQuantity(Guid id, int sign)
        {
            await _orderService.ChangeOrderItemQuantity(id, sign);
            return RedirectToAction("GetCart");
        }
    }
}
