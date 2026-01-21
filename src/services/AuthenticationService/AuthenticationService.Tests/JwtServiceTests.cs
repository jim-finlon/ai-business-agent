using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using AuthenticationService.Models;
using AuthenticationService.Services;
using System.Security.Claims;

namespace AuthenticationService.Tests;

/// <summary>
/// Tests for JwtService functionality
/// </summary>
public class JwtServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<JwtService>> _mockLogger;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<JwtService>>();

        // Setup configuration
        _mockConfiguration.Setup(x => x["Jwt:Secret"])
            .Returns("YourSuperSecretKeyThatIsAtLeast32CharactersLong!");
        _mockConfiguration.Setup(x => x["Jwt:Issuer"])
            .Returns("AuthenticationService");
        _mockConfiguration.Setup(x => x["Jwt:Audience"])
            .Returns("AuthenticationService");
        _mockConfiguration.Setup(x => x["Jwt:ExpirationMinutes"])
            .Returns("60");

        _jwtService = new JwtService(_mockConfiguration.Object, _mockLogger.Object);
    }

    [Fact]
    public void GenerateAccessToken_WithValidUser_ShouldReturnToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Username = "testuser",
            FullName = "Test User",
            IsEmailVerified = true,
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new() { Role = "User" },
                new() { Role = "Admin" }
            }
        };

        // Act
        var token = _jwtService.GenerateAccessToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.Contains(".", token); // JWT tokens have dots
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnBase64String()
    {
        // Act
        var token = _jwtService.GenerateRefreshToken();

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        
        // Should be valid base64
        var bytes = Convert.FromBase64String(token);
        Assert.Equal(64, bytes.Length); // 64 bytes = 512 bits
    }

    [Fact]
    public void ValidateToken_WithValidToken_ShouldReturnPrincipal()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Username = "testuser",
            FullName = "Test User",
            IsEmailVerified = true,
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new() { Role = "User" }
            }
        };

        var token = _jwtService.GenerateAccessToken(user);

        // Act
        var principal = _jwtService.ValidateToken(token);

        // Assert
        Assert.NotNull(principal);
        Assert.True(principal.Identity!.IsAuthenticated);
        
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        Assert.NotNull(userIdClaim);
        Assert.Equal(user.Id.ToString(), userIdClaim.Value);
        
        var emailClaim = principal.FindFirst(ClaimTypes.Email);
        Assert.NotNull(emailClaim);
        Assert.Equal(user.Email, emailClaim.Value);
        
        var roleClaim = principal.FindFirst(ClaimTypes.Role);
        Assert.NotNull(roleClaim);
        Assert.Equal("User", roleClaim.Value);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnNull()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var principal = _jwtService.ValidateToken(invalidToken);

        // Assert
        Assert.Null(principal);
    }

    [Fact]
    public void ValidateRefreshToken_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var validToken = _jwtService.GenerateRefreshToken();

        // Act
        var isValid = _jwtService.ValidateRefreshToken(validToken);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void ValidateRefreshToken_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        var invalidToken = "invalid-token";

        // Act
        var isValid = _jwtService.ValidateRefreshToken(invalidToken);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void ValidateRefreshToken_WithWrongLengthToken_ShouldReturnFalse()
    {
        // Arrange
        var shortToken = Convert.ToBase64String(new byte[32]); // 32 bytes instead of 64

        // Act
        var isValid = _jwtService.ValidateRefreshToken(shortToken);

        // Assert
        Assert.False(isValid);
    }
}
