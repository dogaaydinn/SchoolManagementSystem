using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Menu.AdminMenu;
using SchoolManagementSystem.PresentationLayer.Menu.StudentMenu;
using SchoolManagementSystem.PresentationLayer.Menu.TeacherMenu;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class MenuHandler
{
    public static void DisplayMainMenu(List<Student> students, List<Course> courses, List<Teacher> teachers,
        List<Admin> admins, IUser user)
    {
        while (true)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Admin Menu");
            Console.WriteLine("2. Teacher Menu");
            Console.WriteLine("3. Student Menu");
            Console.WriteLine("4. Exit");
            Console.Write("Select your option: ");

            if (int.TryParse(Console.ReadLine(), out var choice))
                switch (choice)
                {
                    case 1:
                        if (user is Admin)
                            AdminMenu.DisplayAdminMenu(courses, students, teachers, user);
                        else
                            Console.WriteLine("You do not have access to the Admin Menu.");
                        break;
                    case 2:
                        if (user is Teacher)
                            TeacherMenu.DisplayTeacherMenu(teachers, students, courses, user);
                        else
                            Console.WriteLine("You do not have access to the Teacher Menu.");
                        break;
                    case 3:
                        StudentMenu.DisplayStudentMenu(students, courses, user);
                        break;
                    case 4:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please select a valid option.");
                        break;
                }
            else
                Console.WriteLine("Invalid input. Please enter a number.");
        }
    }
}