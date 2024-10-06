using SchoolManagementSystem.BusinessLogicLayer.Authentications;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Abstract;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer;

public static class Program
{
    public static void Main(string[] args)
    {        
        var courses = DataProvider.GetCourses();
 var teachers = DataProvider.GetTeachers();
 var students = DataProvider.GetStudents();
 var admins = DataProvider.GetAdmins();
        var user = GetSchoolMember();
        if (user == null)
        {
            Console.WriteLine("Authentication failed. Exiting...");
            return;
        }



        MenuHandler.DisplayMainMenu(students, courses, teachers, admins, user);
    }

    private static SchoolMember? GetSchoolMember()
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

    private static SchoolMember? AuthenticateSchoolMember()
    {
        Console.Write("Enter first name: ");
        var firstName = Console.ReadLine();
        Console.Write("Enter last name: ");
        var lastName = Console.ReadLine();

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

        var user = Authenticator.Authenticate(firstName, lastName, password, id);
        if (user != null) return user;
        Console.WriteLine("Invalid name, ID, or password.");
        Console.WriteLine("1. Try Again");
        Console.WriteLine("2. Forgot Password");
        Console.Write("Select an option: ");
        var choice = Console.ReadLine();

        if (choice == "2")
        {
            Console.Write("Enter your first name: ");
            firstName = Console.ReadLine();
            Console.Write("Enter your last name: ");
            lastName = Console.ReadLine();
            user = DataProvider.GetSchoolMemberByName(firstName, lastName);
            if (user != null)
                Console.WriteLine($"Your password is: {user.GetPassword()}");
            else
                Console.WriteLine("User not found.");
        }

        return user;
    }

    private static SchoolMember? RegisterSchoolMember()
    {
        Console.Write("Enter first name: ");
        var firstName = Console.ReadLine();
        Console.Write("Enter last name: ");
        var lastName = Console.ReadLine();
        Console.Write("Enter date of birth (yyyy-MM-dd): ");
        var dateOfBirth = DateTime.Parse(Console.ReadLine());

        var newUser = Authenticator.Register(firstName, lastName, dateOfBirth);
        if (newUser == null) return newUser;
        Console.WriteLine(
            $"User created successfully. Your ID is: {newUser.Id}. Your password is: {newUser.GetPassword()}");
        Console.WriteLine("Warning: This information is important. Please note it down.");
        Console.WriteLine("Would you like to change your password? (yes/no)");
        var changePassword = Console.ReadLine();
        if (changePassword?.ToLower() == "yes")
        {
            Console.Write("Enter new password: ");
            var newPassword = Console.ReadLine();
            newUser.SetPassword(newPassword);
            Console.WriteLine("Password changed successfully.");
        }

        Console.WriteLine("Current School Members:");
        foreach (var member in DataProvider.GetAllSchoolMembers()) Console.WriteLine(member.GetFullName());
        return newUser;
    }
}