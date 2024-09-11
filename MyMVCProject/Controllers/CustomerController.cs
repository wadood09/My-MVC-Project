using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Core.Application.Services;
using MyMVCProject.Models.CustomerModel;
using MyMVCProject.Models.ViewModels;
using System.Security.Claims;

namespace MyMVCProject.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly NavigationHistoryManager _historyManager;

        public CustomerController(ICustomerService customerService, NavigationHistoryManager historyManager)
        {
            _customerService = customerService;
            _historyManager = historyManager;
        }

        public IActionResult RegisterCustomer()
        {
            _historyManager.AddToHistory("Customer", nameof(RegisterCustomer));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCustomer(CustomerRequest request)
        {
            var customer = await _customerService.CreateCustomer(request);
            if (customer.IsSuccessful)
            {
                // The crreate page
                TempData["SuccessMessage"] = customer.Message;
                return RedirectToAction("Index", "Home");
            }
            // The list of customers
            TempData["ErrorMessage"] = customer.Message;
            return View(request);
        }

        [Authorize(Roles = "Customer, Admin, Chef")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString()}
            };
            _historyManager.AddToHistory("Customer", nameof(GetCustomer), param);

            var customer = await _customerService.GetCustomer(id);
            if (customer.IsSuccessful)
            {
                // Customer details
                return View(customer.Value);
            }
            // The list of customers
            return RedirectToAction("GetAllCustomers");
        }

        [Authorize(Roles = "Admin, Chef")]
        public async Task<IActionResult> GetAllCustomers()
        {
            _historyManager.AddToHistory("Customer", nameof(GetAllCustomers));

            var customer = await _customerService.GetAllCustomer();
            // The list of customers
            return View(customer.Value);
            //if (customer.Value.Count != 0)
            //{

            //}
            //// The main page
            //return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCustomer(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString()}
            };
            _historyManager.AddToHistory("Customer", nameof(UpdateCustomer), param);

            var customer = await _customerService.GetCustomer(id);
            if (customer.IsSuccessful)
            {
                var updateModel = new UpdateUserViewModel
                {
                    FirstName = customer.Value.FullName.Split(" ")[0],
                    LastName = customer.Value.FullName.Split(" ")[1],
                    Email = customer.Value.Email,
                    Gender = customer.Value.Gender,
                    PhoneNumber = customer.Value.PhoneNumber,
                };
                return View(updateModel);
            }
            // The customer details
            return RedirectToAction("Index", "Customer");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(int id, [FromForm] UpdateCustomerRequest request)
        {
            var customer = await _customerService.UpdateCustomer(id, request);
            if (customer.IsSuccessful)
            {
                TempData["SuccessMessage"] = customer.Message;
                // Customer details
                return RedirectToAction("Index", "Customer");
            }
            TempData["ErrorMessage"] = customer.Message;
            var updateModel = new UpdateUserViewModel
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Email = request.Email,
                Gender = request.Gender,
                ImageUrl = request.Image,
                PhoneNumber = request.PhoneNumber
            };
            // The update page  
            return View(updateModel);
        }

        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString()}
            };
            _historyManager.AddToHistory("Customer", nameof(DeleteCustomer), param);

            var customer = await _customerService.GetCustomer(id);
            if (customer.IsSuccessful)
            {
                // Confirm delete
                return View(customer.Value);
            }
            // Customer Details
            return RedirectToAction("Index", "Customer");
        }

        [HttpPost, ActionName("DeleteCustomer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _customerService.RemoveCustomer(id);
            if (customer.IsSuccessful)
            {
                var role = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Role).Value;
                if (role == "Admin")
                {
                    TempData["SuccessMessage"] = customer.Message;
                    // List of customers
                    return RedirectToAction("GetAllCustomers");
                }
                // Home Page
                return RedirectToAction("Index", "Home");
            }
            TempData["ErrorMessage"] = customer.Message;
            // Customer Details
            return RedirectToAction("Index", "Customer");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
