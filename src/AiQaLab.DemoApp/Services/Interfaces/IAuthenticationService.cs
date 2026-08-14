using AiQaLab.Core.Models;

namespace AiQaLab.DemoApp.Services.Interfaces
{
    public interface IAuthenticationService
    {
        AuthenticationResult Authenticate(string email, string password);
    }
}
