using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Menu;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class MenuHandler
{
    public static void DisplayMainMenu(List<Student> students, List<Course>? courses, List<Teacher> teachers, List<Admin> admins)
    {
        while (true)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Student Operations");
            Console.WriteLine("2. Teacher Operations");
            Console.WriteLine("3. Course Operations");
            Console.WriteLine("4. School Operations");
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
                    StudentMenu.DisplayStudentMenu(students, courses);
                    break;
                case "2":
                    TeacherMenu.DisplayTeacherMenu(teachers, students);
                    break;
                case "3":
                    CourseMenu.DisplayCourseMenu(courses, students);
                    break;
                case "4":
                    Admin admin = admins.FirstOrDefault();
                    if (admin != null)
                    {
                        SchoolMenu.DisplaySchoolMenu(courses, students, teachers, admin);
                    }
                    else
                    {
                        Console.WriteLine("No admin available.");
                    }
                    break;
                case "5":
                    Console.Write("Are you sure you want to exit? (y/n): ");
                    var confirm = Console.ReadLine();
                    if (confirm?.ToLower() == "y")
                    {
                        Console.WriteLine("Exiting...");
                        return;
                    }
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}