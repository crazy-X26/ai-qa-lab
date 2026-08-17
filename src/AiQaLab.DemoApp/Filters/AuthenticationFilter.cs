using AiQaLab.DemoApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AiQaLab.DemoApp.Filters
{
    public class AuthenticationFilter : IAsyncActionFilter
    {
        private readonly IUserSessionService _userSessionService;

        public AuthenticationFilter(IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!_userSessionService.IsAuthenticated())
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            await next();
        }
    }
}
