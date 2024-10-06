using System.Text;
using SchoolManagementSystem.BusinessLogicLayer.Utilities;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Abstract;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.BusinessLogicLayer.Authentications;

public static class Authenticator
{
    private static SchoolMember? Authenticate(string firstName, string lastName, string password, int id)
    {
        var user = DataProvider.GetSchoolMemberByName(firstName, lastName);
        if (user != null && PasswordHelper.ValidatePassword(password, user.GetPassword()) && user.Id == id)
            return user;

        return null;
    }
    private static SchoolMember? AuthenticateSchoolMember()
    {
        Console.Write("Enter first name: ");
        var firstName = Console.ReadLine();
        if (string.IsNullOrEmpty(firstName))
        {
            Console.WriteLine("First name cannot be empty.");
            return null;
        }

        Console.Write("Enter last name: ");
        var lastName = Console.ReadLine();
        if (string.IsNullOrEmpty(lastName))
        {
            Console.WriteLine("Last name cannot be empty.");
            return null;
        }

        int id;
        while (true)
        {
            Console.Write("Enter ID: ");
            var idInput = Console.ReadLine();
            if (int.TryParse(idInput, out id))
                break;
            Console.WriteLine("Invalid ID format. Please enter a numeric ID.");
        }

        Console.Write("Enter password: ");
        var password = Console.ReadLine();
        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return null;
        }

        var user = Authenticate(firstName, lastName, password, id);
        if (user != null)
            return user;

        Console.WriteLine("Invalid name, ID, or password.");
        Console.WriteLine("1. Try Again");
        Console.WriteLine("2. Forgot Password");
        Console.Write("Select an option: ");
        var choice = Console.ReadLine();

        if (choice != "2") return null;
        Console.Write("Enter your first name: ");
        firstName = Console.ReadLine();
        Console.Write("Enter your last name: ");
        lastName = Console.ReadLine();
        user = DataProvider.GetSchoolMemberByName(firstName, lastName);
        if (user != null)
        {
            var fixedPassword = "FixedPassword123"; // Set a fixed password
            var hashedPassword = PasswordHelper.HashPassword(fixedPassword);
            user.SetPassword(hashedPassword);
            Console.WriteLine($"Your password has been reset. Your new password is: {fixedPassword}");
        }
        else
        {
            Console.WriteLine("User not found.");
        }

        return user;
    }
    public static void ResetAllPasswords()
    {
        Console.Write("Enter new password for all users: ");
        var newPassword = Console.ReadLine();
    
        if (string.IsNullOrEmpty(newPassword))
        {
            Console.WriteLine("Password cannot be empty. Operation aborted.");
            return;
        }

        var allUsers = DataProvider.GetAllSchoolMembers();
        var hashedPassword = PasswordHelper.HashPassword(newPassword);

        foreach (var user in allUsers)
        {
            user.SetPassword(hashedPassword);
        }

        Console.WriteLine($"All passwords have been reset to: {newPassword}");
    }

    private static void ChangePassword(SchoolMember user, string newPassword)
    {
        var hashedPassword = PasswordHelper.HashPassword(newPassword);
        user.SetPassword(hashedPassword);
        Console.WriteLine("Password changed successfully.");
    }

    public static SchoolMember? RegisterStudent(string firstName, string lastName, DateTime dateOfBirth)
    {
        var studentId = DataProvider.GenerateStudentId();
        var password = GenerateRandomPassword();
        var hashedPassword = PasswordHelper.HashPassword(password);
        var newStudent = new Student(firstName, lastName, dateOfBirth, studentId, 0, hashedPassword);
        DataProvider.AddSchoolMember(newStudent);

        Console.WriteLine($"User created successfully. Your ID is: {studentId}. Your password is: {password}");
        Console.WriteLine("Warning: This information is important. Please note it down.");
        return newStudent;
    }

    public static void RegisterTeacher(string firstName, string lastName, DateTime dateOfBirth, string subject)
    {
        var password = Authenticator.GenerateRandomPassword();
        var hashedPassword = PasswordHelper.HashPassword(password);
        var teacherId = DataProvider.GenerateTeacherId();
        var newTeacher = new Teacher(firstName, lastName, dateOfBirth, teacherId, subject, hashedPassword);
        DataProvider.AddSchoolMember(newTeacher);
        Console.WriteLine($"Teacher registered successfully. Your password is: {password}");
        Console.WriteLine("Warning: This information is important. Please note it down.");
    }
    public static void RegisterAdmin(string firstName, string lastName, DateTime dateOfBirth)
    {
        var adminId = DataProvider.GenerateAdminId();
        var password = GenerateRandomPassword();
        var hashedPassword = PasswordHelper.HashPassword(password);
        var newAdmin = new Admin(firstName, lastName, dateOfBirth, adminId, hashedPassword);
        DataProvider.AddSchoolMember(newAdmin);

        Console.WriteLine($"Admin created successfully. Your ID is: {adminId}. Your password is: {password}");
        Console.WriteLine("Warning: This information is important. Please note it down.");
    }

    public static string GenerateRandomPassword()
    {
        const int passwordLength = 8;
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        var password = new StringBuilder();
        var random = new Random();

        for (var i = 0; i < passwordLength; i++)
        {
            password.Append(validChars[random.Next(validChars.Length)]);
        }

        return password.ToString();
    }
    public static SchoolMember? GetSchoolMember()
    {
        Console.WriteLine("1. Login");
        Console.WriteLine("2. Register");
        Console.Write("Select an option: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                return AuthenticateSchoolMember();
            case "2":
                return RegisterSchoolMember();
            default:
                Console.WriteLine("Invalid choice.");
                return null;
        }
    }

    private static SchoolMember? RegisterSchoolMember()
    {
        Console.Write("Enter first name: ");
        var firstName = Console.ReadLine();
        Console.Write("Enter last name: ");
        var lastName = Console.ReadLine();
        Console.Write("Enter date of birth (yyyy-MM-dd): ");
        var dateOfBirth = DateTime.Parse(Console.ReadLine());

        var newUser = RegisterStudent(firstName, lastName, dateOfBirth);
        if (newUser == null) return newUser;
        Console.WriteLine("Would you like to change your password? (yes/no)");
        var changePassword = Console.ReadLine();
        if (changePassword?.ToLower() == "yes")
        {
            Console.Write("Enter new password: ");
            var newPassword = Console.ReadLine();
            ChangePassword(newUser, newPassword);
        }

        Console.WriteLine("Current School Members:");
        foreach (var member in DataProvider.GetAllSchoolMembers()) Console.WriteLine(member.GetFullName());
        return newUser;
    }
  
}