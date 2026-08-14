using AiQaLab.DemoApp;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace AiQaLab.Tests.Integration.Sessions
{
    public class LoginTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public LoginTests(WebApplicationFactory<Program> factory)
        {
            var options = new WebApplicationFactoryClientOptions
            {
                HandleCookies = true,
                AllowAutoRedirect = false
            };

            _client = factory.CreateClient(options);
        }

        [Fact]
        public async Task GetLogin_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/Account/Login");
            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Login_WithValidCredentials_RedirectsToHome()
        {
            // Arrange
            var formData = new Dictionary<string, string>
            {
                ["Email"] = "admin@northwind.com",
                ["Password"] = "Password123!"
            };

            // Act
            var response = await _client.PostAsync(
                "/Account/Login",
                new FormUrlEncodedContent(formData));

            var homeresponse = await _client.GetAsync("/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location!.ToString().Should().Be("/");

            homeresponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AccessingHomeWithoutAuthentication_RedirectsToLogin()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location!.ToString()
                .Should().Be("/Account/Login");
        }

        [Fact]
        public async Task AccessingHomeAfterLogout_RedirectsToLogin()
        {
            // Arrange
            var formData = new Dictionary<string, string>
            {
                ["Email"] = "admin@northwind.com",
                ["Password"] = "Password123!"
            };

            // Act
            var response = await _client.PostAsync(
                "/Account/Login",
                new FormUrlEncodedContent(formData));

            var logoutResponse = await _client.PostAsync("/Account/Logout", null);

            var homeResponse = await _client.GetAsync("/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location!.ToString().Should().Be("/");

            logoutResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);
            logoutResponse.Headers.Location!.ToString().Should().Be("/Account/Login");

            homeResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);
            homeResponse.Headers.Location!.ToString().Should().Be("/Account/Login");
        }
    }
}
