using AuthenticationService.DTOs;
using AuthenticationService.Services;
using AuthenticationService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Tests;

/// <summary>
/// Tests for AuthService registration functionality
/// </summary>
public class AuthServiceRegistrationTests : AuthServiceTestBase
{
    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Username = "newuser",
            Password = "NewPassword123!",
            FullName = "New User"
        };

        // Act
        var result = await AuthService.RegisterAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("newuser@example.com", result.Data.User.Email);
        Assert.Equal("newuser", result.Data.User.Username);
        Assert.Equal("New User", result.Data.User.FullName);
        Assert.Contains("User", result.Data.User.Roles);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldFail()
    {
        // Arrange
        await CreateTestUserAsync("existing@example.com", "existinguser");
        var request = new RegisterRequest
        {
            Email = "existing@example.com",
            Username = "newuser",
            Password = "NewPassword123!",
            FullName = "New User"
        };

        // Act
        var result = await AuthService.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("already exists", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUsername_ShouldFail()
    {
        // Arrange
        await CreateTestUserAsync("new@example.com", "existinguser");
        var request = new RegisterRequest
        {
            Email = "new@example.com",
            Username = "existinguser",
            Password = "NewPassword123!",
            FullName = "New User"
        };

        // Act
        var result = await AuthService.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("already exists", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidEmail_ShouldFail()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "invalid-email",
            Username = "newuser",
            Password = "NewPassword123!",
            FullName = "New User"
        };

        // Act
        var result = await AuthService.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("validation", result.Message.ToLower());
    }

    [Fact]
    public async Task RegisterAsync_WithWeakPassword_ShouldFail()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Username = "newuser",
            Password = "123",
            FullName = "New User"
        };

        // Act
        var result = await AuthService.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("validation", result.Message.ToLower());
    }
}

/// <summary>
/// Tests for AuthService login functionality
/// </summary>
public class AuthServiceLoginTests : AuthServiceTestBase
{
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldSucceed()
    {
        // Arrange
        var user = await CreateTestUserAsync("test@example.com", "testuser");
        var request = new LoginRequest
        {
            UsernameOrEmail = "test@example.com",
            Password = "TestPassword123!"
        };

        // Act
        var result = await AuthService.LoginAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("test@example.com", result.Data.User.Email);
        Assert.Equal("testuser", result.Data.User.Username);
        Assert.Equal("mock-access-token", result.Data.AccessToken);
        Assert.Equal("mock-refresh-token", result.Data.RefreshToken);
    }

    [Fact]
    public async Task LoginAsync_WithUsername_ShouldSucceed()
    {
        // Arrange
        var user = await CreateTestUserAsync("test@example.com", "testuser");
        var request = new LoginRequest
        {
            UsernameOrEmail = "testuser",
            Password = "TestPassword123!"
        };

        // Act
        var result = await AuthService.LoginAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("test@example.com", result.Data.User.Email);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldFail()
    {
        // Arrange
        var user = await CreateTestUserAsync("test@example.com", "testuser");
        var request = new LoginRequest
        {
            UsernameOrEmail = "test@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var result = await AuthService.LoginAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Invalid credentials", result.Message);
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ShouldFail()
    {
        // Arrange
        var request = new LoginRequest
        {
            UsernameOrEmail = "nonexistent@example.com",
            Password = "TestPassword123!"
        };

        // Act
        var result = await AuthService.LoginAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Invalid credentials", result.Message);
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ShouldFail()
    {
        // Arrange
        var user = await CreateTestUserAsync("test@example.com", "testuser");
        user.IsActive = false;
        await Context.SaveChangesAsync();

        var request = new LoginRequest
        {
            UsernameOrEmail = "test@example.com",
            Password = "TestPassword123!"
        };

        // Act
        var result = await AuthService.LoginAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("deactivated", result.Message);
    }
}

/// <summary>
/// Tests for AuthService API key functionality
/// </summary>
public class AuthServiceApiKeyTests : AuthServiceTestBase
{
    [Fact]
    public async Task CreateApiKeyAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var request = new CreateApiKeyRequest
        {
            KeyName = "Test API Key",
            Scopes = new[] { "read", "write" },
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await AuthService.CreateApiKeyAsync(user.Id, request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Test API Key", result.Data.KeyName);
        Assert.Equal(new[] { "read", "write" }, result.Data.Scopes);
        Assert.NotNull(result.Data.ApiKey);
        Assert.True(result.Data.ApiKey.StartsWith("ak_"));
    }

    [Fact]
    public async Task CreateApiKeyAsync_WithMaxKeysReached_ShouldFail()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        
        // Create 10 API keys (max limit)
        for (int i = 0; i < 10; i++)
        {
            var apiKey = new ApiKey
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                KeyName = $"Key {i}",
                KeyHash = BCrypt.Net.BCrypt.HashPassword($"key{i}"),
                Prefix = "ak_",
                Scopes = new[] { "read" },
                CreatedAt = DateTime.UtcNow
            };
            Context.ApiKeys.Add(apiKey);
        }
        await Context.SaveChangesAsync();

        var request = new CreateApiKeyRequest
        {
            KeyName = "Should Fail",
            Scopes = new[] { "read" }
        };

        // Act
        var result = await AuthService.CreateApiKeyAsync(user.Id, request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Maximum number of API keys reached", result.Message);
    }

    [Fact]
    public async Task GetUserApiKeysAsync_ShouldReturnUserKeys()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var apiKey = new ApiKey
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            KeyName = "Test Key",
            KeyHash = BCrypt.Net.BCrypt.HashPassword("testkey"),
            Prefix = "ak_",
            Scopes = new[] { "read" },
            CreatedAt = DateTime.UtcNow
        };
        Context.ApiKeys.Add(apiKey);
        await Context.SaveChangesAsync();

        // Act
        var result = await AuthService.GetUserApiKeysAsync(user.Id);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal("Test Key", result.Data[0].KeyName);
    }

    [Fact]
    public async Task RevokeApiKeyAsync_WithValidKey_ShouldSucceed()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var apiKey = new ApiKey
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            KeyName = "Test Key",
            KeyHash = BCrypt.Net.BCrypt.HashPassword("testkey"),
            Prefix = "ak_",
            Scopes = new[] { "read" },
            CreatedAt = DateTime.UtcNow
        };
        Context.ApiKeys.Add(apiKey);
        await Context.SaveChangesAsync();

        // Act
        var result = await AuthService.RevokeApiKeyAsync(user.Id, apiKey.Id);

        // Assert
        Assert.True(result.Success);
        
        var revokedKey = await Context.ApiKeys.FindAsync(apiKey.Id);
        Assert.True(revokedKey!.IsRevoked);
        Assert.NotNull(revokedKey.RevokedAt);
    }

    [Fact]
    public async Task RevokeApiKeyAsync_WithNonExistentKey_ShouldFail()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var nonExistentKeyId = Guid.NewGuid();

        // Act
        var result = await AuthService.RevokeApiKeyAsync(user.Id, nonExistentKeyId);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not found", result.Message);
    }
}

/// <summary>
/// Tests for AuthService user management functionality
/// </summary>
public class AuthServiceUserManagementTests : AuthServiceTestBase
{
    [Fact]
    public async Task GetUserInfoAsync_WithValidUser_ShouldSucceed()
    {
        // Arrange
        var user = await CreateTestUserAsync();

        // Act
        var result = await AuthService.GetUserInfoAsync(user.Id);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(user.Email, result.Data.Email);
        Assert.Equal(user.Username, result.Data.Username);
        Assert.Equal(user.FullName, result.Data.FullName);
        Assert.Contains("User", result.Data.Roles);
    }

    [Fact]
    public async Task GetUserInfoAsync_WithNonExistentUser_ShouldFail()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var result = await AuthService.GetUserInfoAsync(nonExistentUserId);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not found", result.Message);
    }

    [Fact]
    public async Task UpdateUserInfoAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var request = new UpdateUserRequest
        {
            FullName = "Updated Name",
            PhoneNumber = "+1234567890"
        };

        // Act
        var result = await AuthService.UpdateUserInfoAsync(user.Id, request);

        // Assert
        Assert.True(result.Success);
        
        var updatedUser = await Context.Users.FindAsync(user.Id);
        Assert.Equal("Updated Name", updatedUser!.FullName);
        Assert.Equal("+1234567890", updatedUser.PhoneNumber);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithValidCurrentPassword_ShouldSucceed()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "TestPassword123!",
            NewPassword = "NewPassword123!"
        };

        // Act
        var result = await AuthService.ChangePasswordAsync(user.Id, request);

        // Assert
        Assert.True(result.Success);
        
        var updatedUser = await Context.Users.FindAsync(user.Id);
        Assert.True(BCrypt.Net.BCrypt.Verify("NewPassword123!", updatedUser!.PasswordHash));
    }

    [Fact]
    public async Task ChangePasswordAsync_WithInvalidCurrentPassword_ShouldFail()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "WrongPassword123!",
            NewPassword = "NewPassword123!"
        };

        // Act
        var result = await AuthService.ChangePasswordAsync(user.Id, request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("incorrect", result.Message);
    }
}
