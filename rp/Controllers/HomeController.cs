using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace rp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Login()
        {
            return View("Index");
        }

        public IActionResult Logout()
        {
            return SignOut("cookie", "wsfed");
        }
    }
}
