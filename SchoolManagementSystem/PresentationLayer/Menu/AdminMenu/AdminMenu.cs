using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu.AdminMenu;

public static class AdminMenu
{
    public static void DisplayAdminMenu(List<Course>? courses, List<Student?>? students, List<Teacher?>? teachers, object? user)
    {
        while (true)
        {
            Console.WriteLine("\nAdmin Menu:");
            Console.WriteLine("1. Manage Courses");
            Console.WriteLine("2. Manage Students");
            Console.WriteLine("3. Manage Teachers");
            Console.WriteLine("4. View All Details");
            Console.WriteLine("5. Exit");

            Console.Write("Enter your choice: ");

            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                continue;
            }

            switch (choice)
            {
                case "1":
                    AdminCourseMenu.DisplayCourseMenu(courses, students, user);
                    break;
                case "2":
                    AdminStudentMenu.DisplayStudentMenu(students, user);
                    break;
                case "3":
                    AdminTeacherMenu.DisplayTeacherMenu(teachers, courses, user);
                    break;
                case "4":
                    SchoolHandler.DisplayAllDetails(courses, students, teachers, user);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}