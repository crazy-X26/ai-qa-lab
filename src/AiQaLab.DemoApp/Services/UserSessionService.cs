using AiQALab.DemoApp.Services.Interfaces;

namespace AiQALab.DemoApp.Services
{
    public class UserSessionService : IUserSessionService
    {
        public UserSession? GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public bool IsAuthenticated()
        {
            throw new NotImplementedException();
        }

        public void SetCurrentUserSession(string userName, string email)
        {
            throw new NotImplementedException();
        }

        public void SignOut()
        {
            throw new NotImplementedException();
        }
    }
}
