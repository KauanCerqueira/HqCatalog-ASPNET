using Microsoft.AspNetCore.Mvc;

namespace HqCatalog.Mvc.Areas.Site.Controllers
{
    [Area("Site")]
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
