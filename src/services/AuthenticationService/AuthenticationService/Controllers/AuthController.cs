using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuthenticationService.DTOs;
using AuthenticationService.Services;
using System.Security.Claims;

namespace AuthenticationService.Controllers;

/// <summary>
/// Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray();
            return BadRequest(ApiResponse<AuthResponse>.ErrorResult("Validation failed", errors));
        }

        var result = await _authService.RegisterAsync(request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray();
            return BadRequest(ApiResponse<AuthResponse>.ErrorResult("Validation failed", errors));
        }

        var result = await _authService.LoginAsync(request);
        
        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray();
            return BadRequest(ApiResponse<AuthResponse>.ErrorResult("Validation failed", errors));
        }

        var result = await _authService.RefreshTokenAsync(request);
        
        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Logout user
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Logout([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.LogoutAsync(request.RefreshToken);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray();
            return BadRequest(ApiResponse<object>.ErrorResult("Validation failed", errors));
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse<object>.ErrorResult("User not authenticated"));
        }

        var result = await _authService.ChangePasswordAsync(userId.Value, request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserInfo>>> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse<UserInfo>.ErrorResult("User not authenticated"));
        }

        var result = await _authService.GetUserInfoAsync(userId.Value);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Update current user information
    /// </summary>
    [HttpPut("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> UpdateCurrentUser([FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray();
            return BadRequest(ApiResponse<object>.ErrorResult("Validation failed", errors));
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse<object>.ErrorResult("User not authenticated"));
        }

        var result = await _authService.UpdateUserInfoAsync(userId.Value, request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new API key
    /// </summary>
    [HttpPost("api-keys")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CreateApiKeyResponse>>> CreateApiKey([FromBody] CreateApiKeyRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray();
            return BadRequest(ApiResponse<CreateApiKeyResponse>.ErrorResult("Validation failed", errors));
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse<CreateApiKeyResponse>.ErrorResult("User not authenticated"));
        }

        var result = await _authService.CreateApiKeyAsync(userId.Value, request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get user's API keys
    /// </summary>
    [HttpGet("api-keys")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ApiKeyInfo[]>>> GetApiKeys()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse<ApiKeyInfo[]>.ErrorResult("User not authenticated"));
        }

        var result = await _authService.GetUserApiKeysAsync(userId.Value);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Revoke an API key
    /// </summary>
    [HttpDelete("api-keys/{apiKeyId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> RevokeApiKey(Guid apiKeyId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse<object>.ErrorResult("User not authenticated"));
        }

        var result = await _authService.RevokeApiKeyAsync(userId.Value, apiKeyId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Reset password (send reset email)
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray();
            return BadRequest(ApiResponse<object>.ErrorResult("Validation failed", errors));
        }

        var result = await _authService.ResetPasswordAsync(request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Confirm password reset
    /// </summary>
    [HttpPost("confirm-reset-password")]
    public async Task<ActionResult<ApiResponse<object>>> ConfirmResetPassword([FromBody] ConfirmResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray();
            return BadRequest(ApiResponse<object>.ErrorResult("Validation failed", errors));
        }

        var result = await _authService.ConfirmResetPasswordAsync(request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
