using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using AuthenticationService.Services;
using AuthenticationService.Models;

namespace AuthenticationService.Tests;

/// <summary>
/// Base class for authentication service tests
/// </summary>
public abstract class AuthServiceTestBase : IDisposable
{
    protected AuthDbContext Context { get; }
    protected Mock<IJwtService> MockJwtService { get; }
    protected Mock<ILogger<AuthService>> MockLogger { get; }
    protected AuthService AuthService { get; }

    protected AuthServiceTestBase()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new AuthDbContext(options);
        Context.Database.EnsureCreated();

        // Setup mocks
        MockJwtService = new Mock<IJwtService>();
        MockLogger = new Mock<ILogger<AuthService>>();

        // Setup JWT service mock
        MockJwtService.Setup(x => x.GenerateAccessToken(It.IsAny<User>()))
            .Returns("mock-access-token");
        MockJwtService.Setup(x => x.GenerateRefreshToken())
            .Returns("mock-refresh-token");

        // Create service instance
        AuthService = new AuthService(Context, MockJwtService.Object, MockLogger.Object);
    }

    protected async Task<User> CreateTestUserAsync(string email = "test@example.com", string username = "testuser")
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("TestPassword123!"),
            FullName = "Test User",
            IsEmailVerified = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        user.UserRoles.Add(new UserRole { UserId = user.Id, Role = "User" });

        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        return user;
    }

    protected async Task<User> CreateAdminUserAsync(string email = "admin@example.com", string username = "admin")
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"),
            FullName = "Admin User",
            IsEmailVerified = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        user.UserRoles.Add(new UserRole { UserId = user.Id, Role = "Admin" });
        user.UserRoles.Add(new UserRole { UserId = user.Id, Role = "User" });

        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        return user;
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
