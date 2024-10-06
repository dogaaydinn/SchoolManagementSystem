using SchoolManagementSystem.BusinessLogicLayer.Authentications;

namespace SchoolManagementSystem.PresentationLayer.Menu.AdminMenu;

public static class AddMemberMenu
{
    public static void DisplayAddMemberMenu()
    {
        Console.WriteLine("Select member type to add:");
        Console.WriteLine("1. Admin");
        Console.WriteLine("2. Teacher");
        Console.WriteLine("3. Student");
        Console.Write("Enter your choice: ");
        var choice = Console.ReadLine();

        Console.Write("Enter first name: ");
        var firstName = Console.ReadLine();
        Console.Write("Enter last name: ");
        var lastName = Console.ReadLine();
        Console.Write("Enter date of birth (yyyy-MM-dd): ");
        var dateOfBirth = DateTime.Parse(Console.ReadLine());

        switch (choice)
        {
            case "1":
                Authenticator.RegisterAdmin(firstName, lastName, dateOfBirth);
                break;
            case "2":
                Console.Write("Enter subject: ");
                var subject = Console.ReadLine();
                Authenticator.RegisterTeacher(firstName, lastName, dateOfBirth, subject);
                break;
            case "3":
                Authenticator.RegisterStudent(firstName, lastName, dateOfBirth);
                break;
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
}