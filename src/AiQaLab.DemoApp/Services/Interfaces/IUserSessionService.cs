namespace AiQALab.DemoApp.Services.Interfaces
{
    public interface IUserSessionService
    {
        bool IsAuthenticated();

        void SetCurrentUserSession(string userName, string email);

        void SignOut();

        UserSession? GetCurrentUser();
    }
}
