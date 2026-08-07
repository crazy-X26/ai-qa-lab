using AiQALab.DemoApp.Services.Interfaces;
using AiQALab.DemoApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AiQALab.DemoApp.Controllers
{
    public class AccountController : Controller
    {
        private const string DemoUserEmail = "admin@northwind.com";
        private const string DemoUserPassword = "Password123!";

        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            
            var result = _authenticationService.Authenticate(model.Email, model.Password);
            if (result.IsAuthenticated)
            {
                // Handle successful authentication (e.g., set cookies, redirect)
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return View(model);
        }
    }
}
