using System.Security.Cryptography;
using System.Text;

namespace SchoolManagementSystem.BusinessLogicLayer.Utilities;

public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    public static bool ValidatePassword(string enteredPassword, string storedHashedPassword)
    {
        var hashedEnteredPassword = HashPassword(enteredPassword);
        return hashedEnteredPassword == storedHashedPassword;
    }
}