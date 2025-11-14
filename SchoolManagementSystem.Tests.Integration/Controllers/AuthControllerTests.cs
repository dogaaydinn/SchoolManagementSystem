using FluentAssertions;
using SchoolManagementSystem.Core.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace SchoolManagementSystem.Tests.Integration.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            FirstName = "Test",
            LastName = "User",
            Role = "Student"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            Password = "weak",
            ConfirmPassword = "weak",
            FirstName = "Test",
            LastName = "User",
            Role = "Student"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithMismatchedPasswords_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            Password = "TestPassword123!",
            ConfirmPassword = "DifferentPassword123!",
            FirstName = "Test",
            LastName = "User",
            Role = "Student"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange - First register a user
        var registerRequest = new RegisterRequestDto
        {
            Email = $"logintest{Guid.NewGuid()}@example.com",
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            FirstName = "Login",
            LastName = "Test",
            Role = "Student"
        };

        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        var loginRequest = new LoginRequestDto
        {
            EmailOrUsername = registerRequest.Email,
            Password = registerRequest.Password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            EmailOrUsername = "nonexistent@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCurrentUser_WithValidToken_ReturnsUserInfo()
    {
        // Arrange - Register and login
        var email = $"currentuser{Guid.NewGuid()}@example.com";
        var registerRequest = new RegisterRequestDto
        {
            Email = email,
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!",
            FirstName = "Current",
            LastName = "User",
            Role = "Student"
        };

        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        var loginRequest = new LoginRequestDto
        {
            EmailOrUsername = email,
            Password = "TestPassword123!"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();

        // Add authorization header
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Data!.Token);

        // Act
        var response = await _client.GetAsync("/api/v1/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
