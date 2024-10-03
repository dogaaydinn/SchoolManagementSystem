using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Menu.AdminMenu;
using SchoolManagementSystem.PresentationLayer.Menu.StudentMenu;
using SchoolManagementSystem.PresentationLayer.Menu.TeacherMenu;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class MenuHandler
{
    public static void DisplayMainMenu(List<Student?>? students, List<Course>? courses, List<Teacher?>? teachers, List<Admin> admins, object? user)
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

            if (user is not Interfaces.User.IUser validUser)
            {
                Console.WriteLine("Invalid user type.");
                continue;
            }

            switch (choice)
            {
                case "1":
                    if (courses == null)
                    {
                        Console.WriteLine("Courses list is null. Cannot display student menu.");
                    }
                    else
                    {
                        StudentMenu.DisplayStudentMenu(students, courses, validUser);
                    }
                    break;
                case "2":
                    TeacherMenu.DisplayTeacherMenu(teachers, students?.Where(s => s != null).Cast<Student>().ToList(), courses, validUser);
                    break;
                case "3":
                    if (courses == null)
                    {
                        Console.WriteLine("Courses list is null. Cannot display course menu.");
                    }
                    else
                    {
                        CourseMenu.DisplayCourseMenu(courses, students?.Where(s => s != null).Cast<Student>().ToList(), validUser);
                    }
                    break;
                case "4":
                    var admin = admins.FirstOrDefault();
                    if (admin == null)
                    {
                        Console.WriteLine("No admin found. Cannot display school menu.");
                        break;
                    }

                    SchoolMenu.DisplaySchoolMenu(courses, students?.Where(s => s != null).Cast<Student>().ToList(), teachers, admin);
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