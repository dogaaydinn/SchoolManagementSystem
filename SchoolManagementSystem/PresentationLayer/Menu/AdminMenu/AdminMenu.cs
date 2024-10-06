using SchoolManagementSystem.BusinessLogicLayer.Authentications;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;
using SchoolManagementSystem.PresentationLayer.Helpers;

namespace SchoolManagementSystem.PresentationLayer.Menu.AdminMenu;

public static class AdminMenu
{
    public static void DisplayAdminMenu(List<Course> courses, List<Student?>? students, List<Teacher?> teachers, object user)
    {
        var schoolHelper = new SchoolHelper(); 

        while (true)
        {
            Console.WriteLine("\nAdmin Menu:");
            DisplayMenuOptions();

            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                continue;
            }

            if (!HandleMenuChoice(choice, courses, students, teachers, user, schoolHelper)) return;
        }
    }

    private static void DisplayMenuOptions()
    {
        Console.WriteLine("1. Manage Courses");
        Console.WriteLine("2. Manage Students");
        Console.WriteLine("3. Manage Teachers");
        Console.WriteLine("4. View All Details");
        Console.WriteLine("5. Add Member To School");
        Console.WriteLine("6. Reset All Passwords");
        Console.WriteLine("7. Exit");
        Console.Write("Enter your choice: ");
    }

    private static bool HandleMenuChoice(string choice, List<Course> courses, List<Student?>? students, List<Teacher?> teachers, object user, SchoolHelper schoolHelper)
    {
        switch (choice)
        {
            case "1":
                if (user is IUser validUser)
                    AdminCourseMenu.DisplayCourseMenu(courses, students, validUser);
                else
                    Console.WriteLine("Invalid user. Please try again.");
                break;
            case "2":
                AdminStudentMenu.DisplayStudentMenu(students, user);
                break;
            case "3":
                AdminTeacherMenu.DisplayTeacherMenu(teachers, courses, user);
                break;
            case "4":
                SchoolHandler.DisplayAllDetails(schoolHelper, courses, students, teachers, user);
                break;
            case "5":
                AddMemberMenu.DisplayAddMemberMenu();
                break;
            case "6":
                Authenticator.ResetAllPasswords();
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }

        return true;
    }
}