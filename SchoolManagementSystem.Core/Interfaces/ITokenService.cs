using System.Security.Claims;

namespace SchoolManagementSystem.Core.Interfaces;

/// <summary>
/// Token service interface for JWT token generation
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    Task<bool> ValidateTokenAsync(string token);
}
