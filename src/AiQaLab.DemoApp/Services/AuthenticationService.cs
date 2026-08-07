using AiQALab.DemoApp.Services.Interfaces;
using AiQALab.Core.Models;

namespace AiQALab.DemoApp.Services
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
