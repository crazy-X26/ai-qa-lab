namespace AiQALab.Core.Models
{
    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; private init; }
        public string? ErrorMessage { get; private init; }
        public string? UserName { get; private init; }
        public string? Email { get; private init; }

        private AuthenticationResult() { }

        public static AuthenticationResult Success(string userName, string email)
        {
            return new AuthenticationResult
            {
                IsAuthenticated = true,
                UserName = userName,
                Email = email
            };
        }

        public static AuthenticationResult Failure(string errorMessage)
        {
            return new AuthenticationResult
            {
                IsAuthenticated = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
