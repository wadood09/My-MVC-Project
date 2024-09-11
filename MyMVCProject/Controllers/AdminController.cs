using Microsoft.AspNetCore.Mvc;

namespace MyMVCProject.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
