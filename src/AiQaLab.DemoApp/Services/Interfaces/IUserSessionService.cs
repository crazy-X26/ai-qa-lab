using AiQALab.Core.Models;

namespace AiQALab.DemoApp.Services.Interfaces
{
    public interface IUserSessionService
    {
        bool IsAuthenticated();

        void SignIn(AuthenticationResult authenticationResult);

        void SignOut();

        UserSession? GetCurrentUser();
    }
}
