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
        private readonly IUserSessionService _userSessionService;

        public AccountController(IAuthenticationService authenticationService, IUserSessionService userSessionService)
        {
            _authenticationService = authenticationService;
            _userSessionService = userSessionService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (_userSessionService.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _authenticationService.Authenticate(model.Email, model.Password);
            if (!result.IsAuthenticated)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);

                return View(model);
            }

            _userSessionService.SignIn(result);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            _userSessionService.SignOut();
            return RedirectToAction("Login");
        }
    }
}
