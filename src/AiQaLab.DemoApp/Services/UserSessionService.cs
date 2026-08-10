using AiQALab.Core.Models;
using AiQALab.DemoApp.Services.Interfaces;

namespace AiQALab.DemoApp.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string UserNameKey = "UserName";
        private const string EmailKey = "Email";

        public UserSessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserSession? GetCurrentUser()
        {
            var session = _httpContextAccessor.HttpContext?.Session;

            if(session is null) {
                return null;
            }

            var userName = session.GetString(UserNameKey);
            var email = session.GetString(EmailKey);

            if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email)) {
                return null;
            }

            return new UserSession
            {
                UserName = userName,
                Email = email
            };
        }

        public bool IsAuthenticated()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            
            if(session is null) {
                return false;
            }

            var userName = session.GetString(UserNameKey);
            var email = session.GetString(EmailKey);

            return session is not null 
                && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(email);
        }

        public void SignIn(AuthenticationResult authenticationResult)
        {
            if(!authenticationResult.IsAuthenticated)
            {
                throw new ArgumentException("Cannot create a session for an unauthenticated user.", nameof(authenticationResult));
            }

            var session = _httpContextAccessor.HttpContext?.Session;

            if(session is null) {
                throw new InvalidOperationException("Session is not available in the current HTTP context.");
            }

            session.SetString(UserNameKey, authenticationResult.UserName!);
            session.SetString(EmailKey, authenticationResult.Email!);
        }

        public void SignOut()
        {
            var session = _httpContextAccessor.HttpContext?.Session;

            session?.Clear();
        }
    }
}
