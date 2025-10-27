using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;
using AuthenticationService.DTOs;
using AuthenticationService.Services;

namespace AuthenticationService.Tests;

/// <summary>
/// Integration tests for the Authentication API
/// </summary>
public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database
                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "integration@example.com",
            Username = "integrationuser",
            Password = "IntegrationPassword123!",
            FullName = "Integration User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        Assert.NotNull(result);
        Assert.True(result!.Success);
        Assert.Equal("integration@example.com", result.Data!.User.Email);
    }

    [Fact]
    public async Task Register_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "invalid-email",
            Username = "u", // Too short
            Password = "123", // Too weak
            FullName = "Test User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        Assert.NotNull(result);
        Assert.False(result!.Success);
        Assert.Contains("validation", result.Message.ToLower());
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange - First register a user
        var registerRequest = new RegisterRequest
        {
            Email = "login@example.com",
            Username = "loginuser",
            Password = "LoginPassword123!",
            FullName = "Login User"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            UsernameOrEmail = "login@example.com",
            Password = "LoginPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        Assert.NotNull(result);
        Assert.True(result!.Success);
        Assert.NotNull(result.Data!.AccessToken);
        Assert.NotNull(result.Data.RefreshToken);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            UsernameOrEmail = "nonexistent@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        Assert.NotNull(result);
        Assert.False(result!.Success);
        Assert.Contains("Invalid credentials", result.Message);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateApiKey_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new CreateApiKeyRequest
        {
            KeyName = "Test Key",
            Scopes = new[] { "read" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/api-keys", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
