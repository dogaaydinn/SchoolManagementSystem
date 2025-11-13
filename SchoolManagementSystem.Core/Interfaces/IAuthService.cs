using SchoolManagementSystem.Core.DTOs;

namespace SchoolManagementSystem.Core.Interfaces;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(string userId);
    Task<bool> RegisterAsync(RegisterRequestDto request);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request);
    Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequestDto request);
    Task<bool> VerifyEmailAsync(string token);
    Task<string> Generate2FASecretAsync(string userId);
    Task<bool> Enable2FAAsync(string userId, string token);
    Task<bool> Validate2FATokenAsync(string userId, string token);
}
