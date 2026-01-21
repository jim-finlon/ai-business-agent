using Microsoft.EntityFrameworkCore;
using AuthenticationService.Models;
using AuthenticationService.DTOs;
using BCrypt.Net;
using System.Security.Cryptography;

namespace AuthenticationService.Services;

/// <summary>
/// Service for user authentication and management
/// </summary>
public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ApiResponse<object>> LogoutAsync(string refreshToken);
    Task<ApiResponse<object>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordRequest request);
    Task<ApiResponse<object>> ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request);
    Task<ApiResponse<UserInfo>> GetUserInfoAsync(Guid userId);
    Task<ApiResponse<object>> UpdateUserInfoAsync(Guid userId, UpdateUserRequest request);
    Task<ApiResponse<CreateApiKeyResponse>> CreateApiKeyAsync(Guid userId, CreateApiKeyRequest request);
    Task<ApiResponse<ApiKeyInfo[]>> GetUserApiKeysAsync(Guid userId);
    Task<ApiResponse<object>> RevokeApiKeyAsync(Guid userId, Guid apiKeyId);
    Task<ApiResponse<User?>> ValidateApiKeyAsync(string apiKey);
}

public class AuthService : IAuthService
{
    private readonly AuthDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AuthDbContext context, IJwtService jwtService, ILogger<AuthService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.Username == request.Username))
            {
                return ApiResponse<AuthResponse>.ErrorResult("User with this email or username already exists");
            }

            // Create new user
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email.ToLowerInvariant(),
                Username = request.Username.ToLowerInvariant(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                IsEmailVerified = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            // Add default user role
            user.UserRoles.Add(new UserRole { UserId = user.Id, Role = "User" });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Save refresh token
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = MapToUserInfo(user)
            };

            _logger.LogInformation("User registered successfully: {UserId}", user.Id);
            return ApiResponse<AuthResponse>.SuccessResult(response, "User registered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return ApiResponse<AuthResponse>.ErrorResult("An error occurred during registration");
        }
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            // Find user by email or username
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Email == request.UsernameOrEmail.ToLowerInvariant() || 
                                         u.Username == request.UsernameOrEmail.ToLowerInvariant());

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                // Increment failed login attempts
                if (user != null)
                {
                    user.FailedLoginAttempts++;
                    if (user.FailedLoginAttempts >= 5)
                    {
                        user.LockedUntil = DateTime.UtcNow.AddMinutes(30);
                    }
                    await _context.SaveChangesAsync();
                }

                return ApiResponse<AuthResponse>.ErrorResult("Invalid credentials");
            }

            // Check if user is active and not locked
            if (!user.IsActive)
            {
                return ApiResponse<AuthResponse>.ErrorResult("Account is deactivated");
            }

            if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow)
            {
                return ApiResponse<AuthResponse>.ErrorResult("Account is temporarily locked due to too many failed login attempts");
            }

            // Reset failed login attempts
            user.FailedLoginAttempts = 0;
            user.LockedUntil = null;
            user.LastLoginAt = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Save refresh token
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = request.RememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = MapToUserInfo(user)
            };

            _logger.LogInformation("User logged in successfully: {UserId}", user.Id);
            return ApiResponse<AuthResponse>.SuccessResult(response, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return ApiResponse<AuthResponse>.ErrorResult("An error occurred during login");
        }
    }

    public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.IsActive);

            if (refreshToken == null)
            {
                return ApiResponse<AuthResponse>.ErrorResult("Invalid refresh token");
            }

            var user = refreshToken.User;
            if (!user.IsActive)
            {
                return ApiResponse<AuthResponse>.ErrorResult("User account is deactivated");
            }

            // Generate new tokens
            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Revoke old refresh token
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.ReplacedByToken = newRefreshToken;

            // Save new refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(newRefreshTokenEntity);
            await _context.SaveChangesAsync();

            var response = new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = MapToUserInfo(user)
            };

            return ApiResponse<AuthResponse>.SuccessResult(response, "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return ApiResponse<AuthResponse>.ErrorResult("An error occurred during token refresh");
        }
    }

    public async Task<ApiResponse<object>> LogoutAsync(string refreshToken)
    {
        try
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token != null)
            {
                token.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return ApiResponse<object>.SuccessResult(null, "Logout successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return ApiResponse<object>.ErrorResult("An error occurred during logout");
        }
    }

    public async Task<ApiResponse<object>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return ApiResponse<object>.ErrorResult("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return ApiResponse<object>.ErrorResult("Current password is incorrect");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.ModifiedAt = DateTime.UtcNow;

            // Revoke all refresh tokens
            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.IsActive)
                .ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Password changed for user: {UserId}", userId);
            return ApiResponse<object>.SuccessResult(null, "Password changed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password change");
            return ApiResponse<object>.ErrorResult("An error occurred during password change");
        }
    }

    public async Task<ApiResponse<UserInfo>> GetUserInfoAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return ApiResponse<UserInfo>.ErrorResult("User not found");
            }

            return ApiResponse<UserInfo>.SuccessResult(MapToUserInfo(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user info");
            return ApiResponse<UserInfo>.ErrorResult("An error occurred while getting user info");
        }
    }

    public async Task<ApiResponse<User?>> ValidateApiKeyAsync(string apiKey)
    {
        try
        {
            if (string.IsNullOrEmpty(apiKey) || !apiKey.Contains('_'))
            {
                return ApiResponse<User?>.ErrorResult("Invalid API key format");
            }

            var prefix = apiKey.Split('_')[0] + "_";
            var keyHash = BCrypt.Net.BCrypt.HashPassword(apiKey);

            var apiKeyEntity = await _context.ApiKeys
                .Include(ak => ak.User)
                .ThenInclude(u => u.UserRoles)
                .FirstOrDefaultAsync(ak => ak.Prefix == prefix && ak.IsActive);

            if (apiKeyEntity == null || !BCrypt.Net.BCrypt.Verify(apiKey, apiKeyEntity.KeyHash))
            {
                return ApiResponse<User?>.ErrorResult("Invalid API key");
            }

            // Update last used timestamp
            apiKeyEntity.LastUsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ApiResponse<User?>.SuccessResult(apiKeyEntity.User);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating API key");
            return ApiResponse<User?>.ErrorResult("An error occurred while validating API key");
        }
    }

    private UserInfo MapToUserInfo(User user)
    {
        return new UserInfo
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            PhoneNumber = user.PhoneNumber,
            IsEmailVerified = user.IsEmailVerified,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = user.UserRoles.Select(ur => ur.Role).ToArray()
        };
    }

    // Placeholder methods for future implementation
    public async Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        // TODO: Implement password reset with email
        return ApiResponse<object>.ErrorResult("Password reset not implemented yet");
    }

    public async Task<ApiResponse<object>> ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request)
    {
        // TODO: Implement password reset confirmation
        return ApiResponse<object>.ErrorResult("Password reset confirmation not implemented yet");
    }

<<<<<<< HEAD
    public async Task<ApiResponse<CreateApiKeyResponse>> CreateApiKeyAsync(Guid userId, CreateApiKeyRequest request)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return ApiResponse<CreateApiKeyResponse>.ErrorResult("User not found");
            }

            // Check if user has reached the maximum number of API keys
            var existingKeysCount = await _context.ApiKeys
                .CountAsync(ak => ak.UserId == userId && ak.IsActive);
            
            if (existingKeysCount >= 10) // Max 10 API keys per user
            {
                return ApiResponse<CreateApiKeyResponse>.ErrorResult("Maximum number of API keys reached (10)");
            }

            // Generate API key
            var prefix = "ak_";
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var apiKeyValue = prefix + Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");

            var apiKey = new ApiKey
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                KeyName = request.KeyName,
                KeyHash = BCrypt.Net.BCrypt.HashPassword(apiKeyValue),
                Prefix = prefix,
                Scopes = request.Scopes,
                ExpiresAt = request.ExpiresAt,
                CreatedAt = DateTime.UtcNow
            };

            _context.ApiKeys.Add(apiKey);
            await _context.SaveChangesAsync();

            var response = new CreateApiKeyResponse
            {
                Id = apiKey.Id,
                KeyName = apiKey.KeyName,
                Prefix = apiKey.Prefix,
                Scopes = apiKey.Scopes,
                ExpiresAt = apiKey.ExpiresAt,
                LastUsedAt = apiKey.LastUsedAt,
                CreatedAt = apiKey.CreatedAt,
                IsActive = apiKey.IsActive,
                ApiKey = apiKeyValue // Only returned once during creation
            };

            _logger.LogInformation("API key created for user: {UserId}, key: {KeyName}", userId, request.KeyName);
            return ApiResponse<CreateApiKeyResponse>.SuccessResult(response, "API key created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating API key for user: {UserId}", userId);
            return ApiResponse<CreateApiKeyResponse>.ErrorResult("An error occurred while creating API key");
        }
    }

    public async Task<ApiResponse<ApiKeyInfo[]>> GetUserApiKeysAsync(Guid userId)
    {
        try
        {
            var apiKeys = await _context.ApiKeys
                .Where(ak => ak.UserId == userId)
                .OrderByDescending(ak => ak.CreatedAt)
                .Select(ak => new ApiKeyInfo
                {
                    Id = ak.Id,
                    KeyName = ak.KeyName,
                    Prefix = ak.Prefix,
                    Scopes = ak.Scopes,
                    ExpiresAt = ak.ExpiresAt,
                    LastUsedAt = ak.LastUsedAt,
                    CreatedAt = ak.CreatedAt,
                    IsActive = ak.IsActive
                })
                .ToArrayAsync();

            return ApiResponse<ApiKeyInfo[]>.SuccessResult(apiKeys);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting API keys for user: {UserId}", userId);
            return ApiResponse<ApiKeyInfo[]>.ErrorResult("An error occurred while getting API keys");
        }
    }

    public async Task<ApiResponse<object>> RevokeApiKeyAsync(Guid userId, Guid apiKeyId)
    {
        try
        {
            var apiKey = await _context.ApiKeys
                .FirstOrDefaultAsync(ak => ak.Id == apiKeyId && ak.UserId == userId);

            if (apiKey == null)
            {
                return ApiResponse<object>.ErrorResult("API key not found");
            }

            apiKey.IsRevoked = true;
            apiKey.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("API key revoked for user: {UserId}, key: {KeyId}", userId, apiKeyId);
            return ApiResponse<object>.SuccessResult(null, "API key revoked successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking API key for user: {UserId}, key: {KeyId}", userId, apiKeyId);
            return ApiResponse<object>.ErrorResult("An error occurred while revoking API key");
        }
    }

    public async Task<ApiResponse<object>> UpdateUserInfoAsync(Guid userId, UpdateUserRequest request)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return ApiResponse<object>.ErrorResult("User not found");
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;
            
            if (!string.IsNullOrEmpty(request.AvatarUrl))
                user.AvatarUrl = request.AvatarUrl;
            
            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            user.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("User info updated for user: {UserId}", userId);
            return ApiResponse<object>.SuccessResult(null, "User information updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user info for user: {UserId}", userId);
            return ApiResponse<object>.ErrorResult("An error occurred while updating user information");
        }
    }
}

/// <summary>
/// Request DTO for updating user information
/// </summary>
public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? PhoneNumber { get; set; }
}
