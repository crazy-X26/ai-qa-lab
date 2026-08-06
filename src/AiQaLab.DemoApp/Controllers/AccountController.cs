using Microsoft.AspNetCore.Mvc;

namespace AiQALab.DemoApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
