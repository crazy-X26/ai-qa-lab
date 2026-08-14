using AiQaLab.DemoApp.Services;
using FluentAssertions;

namespace AiQaLab.Tests.Unit.Authentication
{
    public class AuthenticationServiceTests
    {
        private readonly AuthenticationService _authenticationService;

        public AuthenticationServiceTests()
        {
            _authenticationService = new AuthenticationService();
        }

        [Fact]
        public void Authenticate_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var email = "admin@northwind.com";
            var password = "Password123!";
            // Act
            var authResult =_authenticationService.Authenticate(email, password);
            // Assert
            authResult.IsAuthenticated.Should().BeTrue();
        }

        [Fact]
        public void Authenticate_WithValidCredentials_ActionResultContainsCorrectUserData()
        {
            // Arrange
            var email = "admin@northwind.com";
            var password = "Password123!";
            // Act
            var authResult =_authenticationService.Authenticate(email, password);
            // Assert
            authResult.IsAuthenticated.Should().BeTrue();
            authResult.Email.Should().Be(email);
            authResult.UserName.Should().Be("Administrator");
        }

        [Theory]
        [InlineData("admin@northwnd.com", "Password123!")]
        [InlineData("admin@northwind.com", "Pasword123!")]
        public void Authenticate_WithInvalidCredentials_ReturnsFailure(string email, string password)
        {
            // Act
            var authResult = _authenticationService.Authenticate(email, password);
            // Assert
            authResult.IsAuthenticated.Should().BeFalse();
        }
    }
}
