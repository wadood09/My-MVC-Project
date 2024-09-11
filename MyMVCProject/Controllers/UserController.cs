using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Core.Application.Services;
using MyMVCProject.Models.UserModel;
using MyMVCProject.Models.ViewModels;
using System.Security.Claims;

namespace MyMVCProject.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly NavigationHistoryManager _historyManager;

        public UserController(IUserService userService, IRoleService roleService, NavigationHistoryManager historyManager)
        {
            _userService = userService;
            _roleService = roleService;
            _historyManager = historyManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            _historyManager.AddToHistory("User", nameof(GetAllUsers));

            var users = await _userService.GetAllUsers();
            if (users.Value.Count > 0)
            {
                return View(users.Value);
            }
            TempData["ErrorMessage"] = "No users have been registered yet";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, Customer, Chef")]
        public async Task<IActionResult> GetUser(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("User", nameof(GetUser), param);

            var user = await _userService.GetUser(id);
            if (user.IsSuccessful)
            {
                return View(user.Value);
            }
            return RedirectToAction("GetAllUsers");
        }

        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> UpdateUser(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("User", nameof(UpdateUser), param);

            var user = await _userService.GetUser(id);
            if (user.IsSuccessful)
            {
                var roles = await _roleService.GetAllRole();
                var viewRoles = roles.Value.TakeWhile(x => x.Name != "Customer").ToList();
                ViewBag.roles = new SelectList(viewRoles, "Id", "Name", viewRoles.Where(x => x.Id == user.Value.RoleId));
                var updateModel = new UpdateUserViewModel
                {
                    FirstName = user.Value.FullName.Split(" ")[0],
                    LastName = user.Value.FullName.Split(" ")[1],
                    Email = user.Value.Email,
                    Gender = user.Value.Gender,
                    PhoneNumber = user.Value.PhoneNumber,
                    RoleId = user.Value.RoleId
                };
                return View(updateModel);
            }
            return RedirectToAction("GetAllUsers");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            var user = await _userService.UpdateUser(id, request);
            if (user.IsSuccessful)
            {
                return RedirectToAction("GetUser", new { id });
            }
            var updateModel = new UpdateUserViewModel
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Email = request.Email,
                Gender = request.Gender,
                ImageUrl = request.ImageUrl,
                PhoneNumber = request.PhoneNumber,
                RoleId = request.RoleId
            };
            TempData["Error"] = user.Message;
            return View(updateModel);
        }

        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("User", nameof(DeleteUser), param);

            var user = await _userService.GetUser(id);
            if (user.IsSuccessful)
            {
                return View(user.Value);
            }
            return RedirectToAction("GetUser", new { id });
        }

        [HttpPost, ActionName("DeleteUser")]
        public async Task<IActionResult> DeleteUserConfirmed(int id)
        {
            var user = await _userService.RemoveUser(id);
            if (user.IsSuccessful)
            {
                return RedirectToAction("GetAllUsers");
            }
            return RedirectToAction("GetUser", new { id });
        }

        [Route("Dashboard")]
        [Authorize(Roles = "Admin, Customer, Chef")]
        public IActionResult Dashboard([FromForm] UserResponse userDTO)
        {
            _historyManager.AddToHistory("User", nameof(Dashboard));

            TempData["Login"] = "true";
            TempData["Role"] = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;
            return View(userDTO);
        }

        [Authorize(Roles = "Chef")]
        public IActionResult StaffDashboard(UserResponse staffDTO)
        {
            _historyManager.AddToHistory("User", nameof(StaffDashboard));

            TempData["Login"] = "true";
            return View(staffDTO);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GoBack(int index = 2)
        {
            var previousView = _historyManager.GetPreviousView(index);
            if (previousView != null)
            {

                return RedirectToAction(previousView.ActionName, previousView.ControllerName, ParseParameters(previousView.Parameters));
            }
            // Redirect to some default view if no previous view is available
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ReView()
        {
            var previousView = _historyManager.GetSameView();
            if (previousView != null)
            {

                return RedirectToAction(previousView.ActionName, previousView.ControllerName, ParseParameters(previousView.Parameters));
            }
            // Redirect to some default view if no previous view is available
            return RedirectToAction("Index", "Home");
        }

        private Dictionary<string, object> ParseParameters(Dictionary<string, string> parameters)
        {
            if (parameters.Count == 0)
                return null;
            Dictionary<string, object> parsedParams = new();
            foreach (var item in parameters)
            {
                dynamic realValue;
                if (int.TryParse(item.Value, out int value))
                {
                    realValue = value;
                }
                else
                {
                    realValue = Guid.Parse(item.Value);
                }
                parsedParams.Add(item.Key, realValue);
            }
            return parsedParams;
        }
    }
}
