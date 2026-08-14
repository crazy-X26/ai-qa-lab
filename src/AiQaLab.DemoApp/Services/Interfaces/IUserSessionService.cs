using AiQaLab.Core.Models;

namespace AiQaLab.DemoApp.Services.Interfaces
{
    public interface IUserSessionService
    {
        bool IsAuthenticated();

        void SignIn(AuthenticationResult authenticationResult);

        void SignOut();

        UserSession? GetCurrentUser();
    }
}
