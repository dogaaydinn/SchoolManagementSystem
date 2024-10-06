using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Menu.TeacherMenu;

public static class TeacherMenu
{
    public static void DisplayTeacherMenu(List<Teacher> teachers, List<Student>? students, List<Course?>? courses,
        object user)
    {
        while (true)
        {
            Console.WriteLine("\nTeacher Menu:");
            DisplayMenuOptions();
            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                continue;
            }

            if (!HandleTeacherMenuChoice(choice, teachers, students, courses, user)) return;
        }
    }

    private static void DisplayMenuOptions()
    {
        Console.WriteLine("1. Manage Teachers");
        Console.WriteLine("2. Manage Students");
        Console.WriteLine("3. Manage Courses");
        Console.WriteLine("4. Exit");
        Console.Write("Enter your choice: ");
    }

    private static bool HandleTeacherMenuChoice(string choice, List<Teacher> teachers, List<Student>? students,
        List<Course?>? courses, object user)
    {
        switch (choice)
        {
            case "1":
                SubTeacherMenu.DisplayTeacherMenu(teachers, user);
                break;
            case "2":
                SubStudentMenu.DisplayStudentMenu(students, user);
                break;
            case "3":
                SubCourseMenu.DisplayCourseMenu(courses, students, user);
                break;
            case "4":
                return false;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }

        return true;
    }
}