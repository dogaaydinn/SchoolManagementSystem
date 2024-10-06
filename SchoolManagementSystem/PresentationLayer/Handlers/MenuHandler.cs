using SchoolManagementSystem.BusinessLogicLayer.Validations;
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
                        try
                        {
                            ValidationHelper.ValidateAdminAccess(user);
                            AdminMenu.DisplayAdminMenu(courses, students, teachers, user);
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case 2:
                        try
                        {
                            ValidationHelper.ValidateUserPermission(user, requireTeacherOrAdmin: true);
                            TeacherMenu.DisplayTeacherMenu(teachers, students, courses, user);
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
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