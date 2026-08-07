using AiQALab.Core.Models;

namespace AiQALab.DemoApp.Services.Interfaces
{
    public interface IAuthenticationService
    {
        AuthenticationResult Authenticate(string email, string password);
    }
}
