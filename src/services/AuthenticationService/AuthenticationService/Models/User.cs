using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Models;

/// <summary>
/// User entity for authentication and authorization
/// </summary>
public class User
{
    public Guid Id { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? FullName { get; set; }
    
    [MaxLength(500)]
    public string? AvatarUrl { get; set; }
    
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    public bool IsEmailVerified { get; set; } = false;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLoginAt { get; set; }
    
    public int FailedLoginAttempts { get; set; } = 0;
    
    public DateTime? LockedUntil { get; set; }
    
    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();
}

/// <summary>
/// User roles for role-based access control
/// </summary>
public class UserRole
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}

/// <summary>
/// Refresh token for JWT token refresh mechanism
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Token { get; set; } = string.Empty;
    
    public DateTime ExpiresAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? RevokedAt { get; set; }
    
    [MaxLength(500)]
    public string? ReplacedByToken { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    
    public bool IsRevoked => RevokedAt.HasValue;
    
    public bool IsActive => !IsRevoked && !IsExpired;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}

/// <summary>
/// API keys for programmatic access
/// </summary>
public class ApiKey
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string KeyName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string KeyHash { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Prefix { get; set; } = string.Empty;
    
    public string[] Scopes { get; set; } = Array.Empty<string>();
    
    public DateTime? ExpiresAt { get; set; }
    
    public DateTime? LastUsedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsRevoked { get; set; } = false;
    
    public DateTime? RevokedAt { get; set; }
    
    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow >= ExpiresAt.Value;
    
    public bool IsActive => !IsRevoked && !IsExpired;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}
