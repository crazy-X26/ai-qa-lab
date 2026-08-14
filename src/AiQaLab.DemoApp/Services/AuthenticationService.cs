using AiQaLab.DemoApp.Services.Interfaces;
using AiQaLab.Core.Models;

namespace AiQaLab.DemoApp.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public AuthenticationResult Authenticate(string email, string password)
        {
            const string DemoUserEmail = "admin@northwind.com";
            const string DemoUserPassword = "Password123!";

            if (string.Equals(email, DemoUserEmail, StringComparison.OrdinalIgnoreCase) && password == DemoUserPassword)
            {
                return AuthenticationResult.Success("Administrator", email);
            }
            
            return AuthenticationResult.Failure("Invalid email or password.");
        }
    }
}
