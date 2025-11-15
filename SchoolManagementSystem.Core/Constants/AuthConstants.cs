namespace SchoolManagementSystem.Core.Constants;

/// <summary>
/// Authentication and authorization related constants
/// </summary>
public static class AuthConstants
{
    /// <summary>
    /// Maximum number of failed login attempts before account lockout
    /// </summary>
    public const int MaxFailedLoginAttempts = 5;

    /// <summary>
    /// Account lockout duration in minutes
    /// </summary>
    public const int AccountLockoutMinutes = 30;

    /// <summary>
    /// Refresh token expiry in days
    /// </summary>
    public const int RefreshTokenExpiryDays = 7;

    /// <summary>
    /// Access token expiry in minutes
    /// </summary>
    public const int AccessTokenExpiryMinutes = 15;

    /// <summary>
    /// Password reset token expiry in hours
    /// </summary>
    public const int PasswordResetTokenExpiryHours = 1;

    /// <summary>
    /// Email verification token expiry in hours
    /// </summary>
    public const int EmailVerificationTokenExpiryHours = 24;

    /// <summary>
    /// 2FA token validity window in seconds
    /// </summary>
    public const int TwoFactorTokenValiditySeconds = 30;
}

/// <summary>
/// Role name constants
/// </summary>
public static class RoleConstants
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Teacher = "Teacher";
    public const string Student = "Student";
    public const string Parent = "Parent";
    public const string Staff = "Staff";

    /// <summary>
    /// Roles that can be self-assigned during public registration
    /// </summary>
    public static readonly HashSet<string> AllowedPublicRegistrationRoles = new()
    {
        Student
    };

    /// <summary>
    /// All valid roles in the system
    /// </summary>
    public static readonly HashSet<string> AllRoles = new()
    {
        SuperAdmin,
        Admin,
        Teacher,
        Student,
        Parent,
        Staff
    };

    /// <summary>
    /// Administrative roles
    /// </summary>
    public static readonly HashSet<string> AdministrativeRoles = new()
    {
        SuperAdmin,
        Admin
    };
}

/// <summary>
/// Custom claim type constants
/// </summary>
public static class ClaimTypeConstants
{
    public const string FirstName = "FirstName";
    public const string LastName = "LastName";
    public const string StudentId = "StudentId";
    public const string TeacherId = "TeacherId";
    public const string AdminId = "AdminId";
    public const string TwoFactorEnabled = "TwoFactorEnabled";
}

/// <summary>
/// Authorization policy name constants
/// </summary>
public static class PolicyConstants
{
    public const string SuperAdminOnly = "SuperAdminOnly";
    public const string AdminOnly = "AdminOnly";
    public const string TeacherOnly = "TeacherOnly";
    public const string StudentOnly = "StudentOnly";
    public const string AdminOrTeacher = "AdminOrTeacher";
}
