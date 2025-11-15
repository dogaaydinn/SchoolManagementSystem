using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;
using SchoolManagementSystem.Infrastructure.Identity;
using Xunit;

namespace SchoolManagementSystem.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<SignInManager<User>> _mockSignInManager;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var userStore = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(
            userStore.Object, null, null, null, null, null, null, null, null);

        _mockSignInManager = new Mock<SignInManager<User>>(
            _mockUserManager.Object,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<User>>(),
            null, null, null, null);

        _mockTokenService = new Mock<ITokenService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _authService = new AuthService(
            _mockUserManager.Object,
            _mockSignInManager.Object,
            _mockTokenService.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidRole_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "TestPass123!",
            ConfirmPassword = "TestPass123!",
            FirstName = "Test",
            LastName = "User",
            Role = "SuperAdmin" // Not allowed for public registration
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.RegisterAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User"
        };

        var loginRequest = new LoginRequestDto
        {
            EmailOrUsername = "test@example.com",
            Password = "TestPass123!"
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Success);

        _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string> { "Student" });

        _mockTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<IEnumerable<System.Security.Claims.Claim>>()))
            .Returns("fake-access-token");

        _mockTokenService.Setup(x => x.GenerateRefreshToken())
            .Returns("fake-refresh-token");

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("fake-access-token");
        result.Data.RefreshToken.Should().Be("fake-refresh-token");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ShouldReturnFailure()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            EmailOrUsername = "nonexistent@example.com",
            Password = "WrongPassword123!"
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid");
    }

    [Fact]
    public async Task LoginAsync_WithLockedAccount_ShouldReturnFailure()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "locked@example.com",
            LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        var loginRequest = new LoginRequestDto
        {
            EmailOrUsername = "locked@example.com",
            Password = "TestPass123!"
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.LockedOut);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("locked");
    }

    // TODO: Add more tests for:
    // - RefreshTokenAsync
    // - ForgotPasswordAsync
    // - ResetPasswordAsync
    // - Enable2FAAsync
    // - Validate2FATokenAsync
    // - VerifyEmailAsync
}
