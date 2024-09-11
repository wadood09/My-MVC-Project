using Microsoft.AspNetCore.Mvc;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Core.Application.Services;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Models.RoleModel;

namespace MyMVCProject.Controllers
{
    public class RoleController : Controller
    {
        public readonly IRoleService _roleService;
        private readonly NavigationHistoryManager _historyManager;

        public RoleController(IRoleService roleService, NavigationHistoryManager historyManager)
        {
            _roleService = roleService;
            _historyManager = historyManager;
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleRequest request)
        {
            var role = await _roleService.CreateRole(request);
            if (role.IsSuccessful)
            {
                TempData["SuccessMessage"] = role.Message;
                return RedirectToAction("GetAllRole");
            }
            TempData["ErrorMessage"] = role.Message;
            return View(request);
        }

        public async Task<IActionResult> GetRole(int id)
        {
            var role = await _roleService.GetRole(id);
            if (role.IsSuccessful)
            {
                return View(role.Value);
            }
            return RedirectToAction("GetAllRole");
        }

        public async Task<IActionResult> GetAllRole()
        {
            var role = await _roleService.GetAllRole();
            if (role.Value.Count != 0)
            {
                return View(role.Value);
            }
            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> UpdateRole(int id)
        {
            var role = await _roleService.GetRole(id);
            if (role.IsSuccessful)
            {
                return View(role.Value);
            }
            return RedirectToAction("GetRole", id);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(int id, UpdateRoleRequest request)
        {
            var role = await _roleService.UpdateRole(id, request);
            if (role.IsSuccessful)
            {
                return RedirectToAction("GetRole", id);
            }
            TempData["ErrorMessage"] = role.Message;
            return View(request);
        }

        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _roleService.GetRole(id);
            if (role.IsSuccessful)
            {
                return View(role.Value);
            }
            return RedirectToAction("GetRole", id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoleConfirmed(int id)
        {
            var role = await _roleService.RemoveRole(id);
            if (role.IsSuccessful)
            {
                TempData["SuccessMessage"] = role.Message;
                return RedirectToAction("GetAllRole");
            }
            return RedirectToAction("GetRole", id);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
