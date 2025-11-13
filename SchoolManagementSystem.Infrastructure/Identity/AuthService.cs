using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Core.DTOs;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;
using BCrypt.Net;

namespace SchoolManagementSystem.Infrastructure.Identity;

/// <summary>
/// Authentication service implementation
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
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid credentials"
            };
        }

        // Check account lockout
        if (user.LockoutEndDate.HasValue && user.LockoutEndDate > DateTime.UtcNow)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = $"Account is locked until {user.LockoutEndDate.Value:yyyy-MM-dd HH:mm:ss} UTC"
            };
        }

        // Verify password
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            // Increment failed login attempts
            user.FailedLoginAttempts++;

            // Lock account after 5 failed attempts
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEndDate = DateTime.UtcNow.AddMinutes(30);
                user.FailedLoginAttempts = 0;
            }

            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                Success = false,
                Message = result.IsLockedOut ? "Account is locked due to multiple failed login attempts" : "Invalid credentials"
            };
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
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid two-factor authentication code"
                };
            }
        }

        // Reset failed login attempts
        user.FailedLoginAttempts = 0;
        user.LastLoginAt = DateTime.UtcNow;

        // Generate tokens
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        // Log audit
        await _unitOfWork.AuditLogs.AddAsync(new AuditLog
        {
            UserId = user.Id,
            Action = "Login",
            EntityType = "User",
            EntityId = user.Id,
            Timestamp = DateTime.UtcNow,
            Severity = "Info",
            Details = $"User {user.Email} logged in successfully"
        });
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login successful",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                Username = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                Age = user.Age,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Roles = roles.ToList(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid or expired refresh token"
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var newAccessToken = _tokenService.GenerateAccessToken(claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Token refreshed successfully",
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
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
        await _unitOfWork.AuditLogs.AddAsync(new AuditLog
        {
            UserId = user.Id,
            Action = "Logout",
            EntityType = "User",
            EntityId = user.Id,
            Timestamp = DateTime.UtcNow,
            Severity = "Info",
            Details = $"User {user.Email} logged out"
        });
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RegisterAsync(RegisterRequestDto request)
    {
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
            IsActive = true,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return false;
        }

        await _userManager.AddToRoleAsync(user, request.Role);

        // Create role-specific entity
        if (request.Role == "Student")
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
        else if (request.Role == "Teacher")
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
        await _unitOfWork.AuditLogs.AddAsync(new AuditLog
        {
            UserId = user.Id,
            Action = "Register",
            EntityType = "User",
            EntityId = user.Id,
            Timestamp = DateTime.UtcNow,
            Severity = "Info",
            Details = $"New user registered: {user.Email} as {request.Role}"
        });
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return false;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

        await _userManager.UpdateAsync(user);

        // TODO: Send email with reset link
        // await _emailService.SendPasswordResetEmailAsync(user.Email, token);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || user.PasswordResetToken != request.Token ||
            user.PasswordResetTokenExpiry < DateTime.UtcNow)
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
        return result.Succeeded;
    }

    public async Task<bool> VerifyEmailAsync(string token)
    {
        // TODO: Implement email verification
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

        // Generate a random secret key for 2FA
        var secret = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        user.TwoFactorSecretKey = secret;
        await _userManager.UpdateAsync(user);

        return secret;
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

        return true;
    }

    public async Task<bool> Validate2FATokenAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.TwoFactorSecretKey))
        {
            return false;
        }

        // TODO: Implement actual TOTP validation
        // This is a placeholder - you would use a library like OtpNet or GoogleAuthenticator
        await Task.CompletedTask;
        return true;
    }

    private string GenerateStudentNumber()
    {
        var year = DateTime.UtcNow.Year;
        var random = new Random().Next(1000, 9999);
        return $"STU{year}{random}";
    }

    private string GenerateEmployeeNumber()
    {
        var year = DateTime.UtcNow.Year;
        var random = new Random().Next(1000, 9999);
        return $"EMP{year}{random}";
    }
}
