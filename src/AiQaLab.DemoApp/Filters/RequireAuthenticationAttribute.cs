using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace AiQaLab.DemoApp.Filters
{
    public class RequireAuthenticationAttribute : TypeFilterAttribute
    {
        public RequireAuthenticationAttribute() : base(typeof(AuthenticationFilter))
        {
        }
    }
}
