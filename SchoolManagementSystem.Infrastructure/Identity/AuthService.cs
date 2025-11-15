using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using SchoolManagementSystem.Core.Constants;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;
using BCrypt.Net;

namespace SchoolManagementSystem.Infrastructure.Identity;

/// <summary>
/// Authentication service implementation with enterprise security features
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Find user by email or username
        var user = await _userManager.Users
            .Include(u => u.Student)
            .Include(u => u.Teacher)
            .Include(u => u.Admin)
            .FirstOrDefaultAsync(u => u.Email == request.EmailOrUsername || u.UserName == request.EmailOrUsername);

        if (user == null || !user.IsActive)
        {
            return AuthResponseDto.Failure("Invalid credentials");
        }

        // Check account lockout
        if (user.LockoutEndDate.HasValue && user.LockoutEndDate > DateTime.UtcNow)
        {
            return AuthResponseDto.Failure($"Account is locked until {user.LockoutEndDate.Value:yyyy-MM-dd HH:mm:ss} UTC");
        }

        // Verify password
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            // Increment failed login attempts
            user.FailedLoginAttempts++;

            // Lock account after max failed attempts using constant
            if (user.FailedLoginAttempts >= AuthConstants.MaxFailedLoginAttempts)
            {
                user.LockoutEndDate = DateTime.UtcNow.AddMinutes(AuthConstants.AccountLockoutMinutes);
                user.FailedLoginAttempts = 0;
            }

            await _userManager.UpdateAsync(user);

            return AuthResponseDto.Failure(result.IsLockedOut
                ? "Account is locked due to multiple failed login attempts"
                : "Invalid credentials");
        }

        // Check 2FA
        if (user.TwoFactorEnabled)
        {
            if (string.IsNullOrEmpty(request.TwoFactorCode))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    RequiresTwoFactor = true,
                    Message = "Two-factor authentication code required"
                };
            }

            var is2FaValid = await Validate2FATokenAsync(user.Id.ToString(), request.TwoFactorCode);
            if (!is2FaValid)
            {
                return AuthResponseDto.Failure("Invalid two-factor authentication code");
            }
        }

        // Reset failed login attempts
        user.FailedLoginAttempts = 0;
        user.LastLoginAt = DateTime.UtcNow;

        // Generate tokens using helper method
        var roles = await _userManager.GetRolesAsync(user);
        var claims = BuildUserClaims(user, roles);

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(AuthConstants.RefreshTokenExpiryDays);

        await _userManager.UpdateAsync(user);

        // Log audit
        await LogAuditAsync(user.Id, "Login", "User", user.Id,
            $"User {user.Email} logged in successfully");

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login successful",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(AuthConstants.AccessTokenExpiryMinutes),
            User = MapToUserDto(user, roles)
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return AuthResponseDto.Failure("Invalid or expired refresh token");
        }

        // Generate new tokens using helper method
        var roles = await _userManager.GetRolesAsync(user);
        var claims = BuildUserClaims(user, roles);

        var newAccessToken = _tokenService.GenerateAccessToken(claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(AuthConstants.RefreshTokenExpiryDays);

        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Token refreshed successfully",
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(AuthConstants.AccessTokenExpiryMinutes)
        };
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _userManager.UpdateAsync(user);

        // Log audit
        await LogAuditAsync(user.Id, "Logout", "User", user.Id, $"User {user.Email} logged out");

        return true;
    }

    public async Task<bool> RegisterAsync(RegisterRequestDto request)
    {
        // CRITICAL SECURITY FIX: Validate role against allowed public registration roles
        if (!RoleConstants.AllowedPublicRegistrationRoles.Contains(request.Role))
        {
            throw new UnauthorizedAccessException(
                $"Role '{request.Role}' is not allowed for public registration. Only {string.Join(", ", RoleConstants.AllowedPublicRegistrationRoles)} roles can be self-registered.");
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return false;
        }

        var user = new User
        {
            Email = request.Email,
            UserName = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return false;
        }

        await _userManager.AddToRoleAsync(user, request.Role);

        // Create role-specific entity using constants
        if (request.Role == RoleConstants.Student)
        {
            var student = new Student
            {
                UserId = user.Id,
                StudentNumber = GenerateStudentNumber(),
                EnrollmentDate = DateTime.UtcNow,
                Status = "Active"
            };
            await _unitOfWork.Students.AddAsync(student);
        }
        else if (request.Role == RoleConstants.Teacher)
        {
            var teacher = new Teacher
            {
                UserId = user.Id,
                EmployeeNumber = GenerateEmployeeNumber(),
                HireDate = DateTime.UtcNow,
                IsActive = true
            };
            await _unitOfWork.Teachers.AddAsync(teacher);
        }

        await _unitOfWork.SaveChangesAsync();

        // Log audit
        await LogAuditAsync(user.Id, "Register", "User", user.Id,
            $"New user registered: {user.Email} as {request.Role}");

        return true;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Return true even if user doesn't exist to prevent email enumeration
            return true;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // SECURITY FIX: Hash the token before storing
        user.PasswordResetToken = BCrypt.HashPassword(token, workFactor: 12);
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(AuthConstants.PasswordResetTokenExpiryHours);

        await _userManager.UpdateAsync(user);

        // TODO: Send email with reset link
        // await _emailService.SendPasswordResetEmailAsync(user.Email, token);

        // Log audit
        await LogAuditAsync(user.Id, "ForgotPassword", "User", user.Id,
            $"Password reset requested for {user.Email}");

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null ||
            string.IsNullOrEmpty(user.PasswordResetToken) ||
            user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            return false;
        }

        // SECURITY FIX: Verify hashed token
        if (!BCrypt.Verify(request.Token, user.PasswordResetToken))
        {
            return false;
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            return false;
        }

        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        await _userManager.UpdateAsync(user);

        // Log audit
        await LogAuditAsync(user.Id, "ResetPassword", "User", user.Id,
            $"Password reset completed for {user.Email}");

        return true;
    }

    public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequestDto request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
        {
            await LogAuditAsync(user.Id, "ChangePassword", "User", user.Id,
                $"Password changed for {user.Email}");
        }

        return result.Succeeded;
    }

    public async Task<bool> VerifyEmailAsync(string token)
    {
        // TODO: Implement email verification
        // Find user by email verification token
        // Verify token hasn't expired
        // Set EmailConfirmed = true
        await Task.CompletedTask;
        return true;
    }

    public async Task<string> Generate2FASecretAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // SECURITY FIX: Generate cryptographically secure secret using OtpNet
        var key = KeyGeneration.GenerateRandomKey(20); // 160-bit key
        var base32Secret = Base32Encoding.ToString(key);

        user.TwoFactorSecretKey = base32Secret;
        await _userManager.UpdateAsync(user);

        return base32Secret;
    }

    public async Task<bool> Enable2FAAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        // Validate the provided token
        var isValid = await Validate2FATokenAsync(userId, token);
        if (!isValid)
        {
            return false;
        }

        user.TwoFactorEnabled = true;
        await _userManager.UpdateAsync(user);

        // Log audit
        await LogAuditAsync(user.Id, "Enable2FA", "User", user.Id,
            $"Two-factor authentication enabled for {user.Email}");

        return true;
    }

    public async Task<bool> Validate2FATokenAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.TwoFactorSecretKey))
        {
            return false;
        }

        // SECURITY FIX: Implement real TOTP validation using OtpNet
        try
        {
            var secretBytes = Base32Encoding.ToBytes(user.TwoFactorSecretKey);
            var totp = new Totp(secretBytes, step: 30); // 30-second window

            // Verify with a time window to account for clock drift
            var verificationWindow = new VerificationWindow(
                previous: 1,
                future: 1
            );

            var isValid = totp.VerifyTotp(
                token,
                out long timeStepMatched,
                verificationWindow
            );

            return isValid;
        }
        catch
        {
            return false;
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Builds JWT claims for a user
    /// </summary>
    private List<Claim> BuildUserClaims(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypeConstants.FirstName, user.FirstName),
            new Claim(ClaimTypeConstants.LastName, user.LastName),
            new Claim(ClaimTypeConstants.TwoFactorEnabled, user.TwoFactorEnabled.ToString())
        };

        // Add role claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Add role-specific claims
        if (user.Student != null)
        {
            claims.Add(new Claim(ClaimTypeConstants.StudentId, user.Student.Id.ToString()));
        }
        if (user.Teacher != null)
        {
            claims.Add(new Claim(ClaimTypeConstants.TeacherId, user.Teacher.Id.ToString()));
        }
        if (user.Admin != null)
        {
            claims.Add(new Claim(ClaimTypeConstants.AdminId, user.Admin.Id.ToString()));
        }

        return claims;
    }

    /// <summary>
    /// Maps User entity to UserDto
    /// </summary>
    private UserDto MapToUserDto(User user, IList<string> roles)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            Username = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            DateOfBirth = user.DateOfBirth,
            Age = user.Age,
            PhoneNumber = user.PhoneNumber,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Roles = roles.ToList(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            TwoFactorEnabled = user.TwoFactorEnabled
        };
    }

    /// <summary>
    /// Logs an audit entry
    /// </summary>
    private async Task LogAuditAsync(int userId, string action, string entityType, int entityId, string details)
    {
        await _unitOfWork.AuditLogs.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Timestamp = DateTime.UtcNow,
            Severity = "Info",
            Details = details
        });
        await _unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Generates a unique student number using cryptographically secure random number generator
    /// </summary>
    private string GenerateStudentNumber()
    {
        var year = DateTime.UtcNow.Year;
        // SECURITY FIX: Use RandomNumberGenerator instead of Random
        var random = RandomNumberGenerator.GetInt32(1000, 10000);
        return $"STU{year}{random}";
    }

    /// <summary>
    /// Generates a unique employee number using cryptographically secure random number generator
    /// </summary>
    private string GenerateEmployeeNumber()
    {
        var year = DateTime.UtcNow.Year;
        // SECURITY FIX: Use RandomNumberGenerator instead of Random
        var random = RandomNumberGenerator.GetInt32(1000, 10000);
        return $"EMP{year}{random}";
    }

    #endregion
}
