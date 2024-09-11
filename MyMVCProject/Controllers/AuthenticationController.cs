using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Core.Application.Services;
using MyMVCProject.Models.UserModel;
using System.Security.Claims;

namespace MyMVCProject.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly NavigationHistoryManager _historyManager;

        public AuthenticationController(IUserService userService, IRoleService roleService, NavigationHistoryManager historyManager)
        {
            _userService = userService;
            _roleService = roleService;
            _historyManager = historyManager;
        }

        public IActionResult Login()
        {
            _historyManager.AddToHistory("Authentication", nameof(Login));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginRequestModel request)
        {
            var login = await _userService.Login(request);
            if (!login.IsSuccessful)
            {
                TempData["ErrorMessage"] = login.Message;
                return View(request);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, login.Value.Id.ToString()),
                new Claim(ClaimTypes.Name, login.Value.FullName),
                new Claim(ClaimTypes.Email, login.Value.Email.ToString()),
                new Claim(ClaimTypes.MobilePhone, login.Value.PhoneNumber),
                new Claim("Age", login.Value.Age.ToString()),
                new Claim(ClaimTypes.Role, login.Value.RoleName),
                new Claim("Profile", login.Value.ImageUrl ?? "bb")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties();

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

            TempData["Profile"] = login.Value.ImageUrl;

            return RedirectToAction("Dashboard", "User", login.Value);
        }

        public async Task<IActionResult> Register()
        {
            _historyManager.AddToHistory("Authentication", nameof(Register));

            var roles = await _roleService.GetAllRole();
            var viewRoles = roles.Value.TakeWhile(x => x.Name != "Customer").ToList();
            ViewBag.roles = new SelectList(viewRoles, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRequest request)
        {
            var user = await _userService.CreateUser(request);
            if (user.IsSuccessful)
            {
                return RedirectToAction("GetAllUsers", "User");
            }
            var roles = await _roleService.GetAllRole();
            var viewRoles = roles.Value.TakeWhile(x => x.Name != "Customer").ToList();
            ViewBag.roles = new SelectList(viewRoles, "Id", "Name");
            TempData["ErrorMessage"] = user.Message;
            return View(request);
        }

        public async Task<IActionResult> Logout()
        {
            TempData["Login"] = "true";
            await HttpContext.SignOutAsync();
            TempData["Login"] = null;
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
