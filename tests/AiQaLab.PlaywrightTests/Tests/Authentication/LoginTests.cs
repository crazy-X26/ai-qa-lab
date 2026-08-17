using AiQaLab.PlaywrightTests.Infrastructure;
using Microsoft.Playwright;

namespace AiQaLab.PlaywrightTests.Tests.Authentication
{
    public class LoginTests : PlaywrightTestBase
    {
        [Fact]
        public async Task LoginPage_IsDisplayed()
        {
            //Act
            await Page.GotoAsync($"{BaseUrl}Account/Login");

            //Assert
            await Assertions.Expect(Page).ToHaveTitleAsync("Login - Northwind Logistics");
        }

        [Fact]
        public async Task Login_WithValidCredentials_RedirectsToHome()
        {
            //Act
            await Page.GotoAsync($"{BaseUrl}Account/Login");

            await Page.GetByTestId("login-email").FillAsync("admin@northwind.com");
            await Page.GetByTestId("login-password").FillAsync("Password123!");
            await Page.GetByTestId("login-submit").ClickAsync();

            //Assert
            await Assertions.Expect(Page).ToHaveURLAsync($"{BaseUrl}");
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShowsErrorMessage()
        {
            
            //Act
            await Page.GotoAsync($"{BaseUrl}Account/Login");

            await Page.GetByTestId("login-email").FillAsync("invalid@northwind.com");
            await Page.GetByTestId("login-password").FillAsync("InvalidPassword!");
            await Page.GetByTestId("login-submit").ClickAsync();

            //Assert
           await Assertions.Expect(Page.GetByTestId("login-error-message")).ToContainTextAsync("Invalid email or password.");
        }
    }
}
