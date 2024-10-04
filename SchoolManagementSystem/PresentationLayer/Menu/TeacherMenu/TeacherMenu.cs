using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;
using SchoolManagementSystem.PresentationLayer.Helpers;

namespace SchoolManagementSystem.PresentationLayer.Menu.TeacherMenu;

public static class TeacherMenu
{
    public static void DisplayTeacherMenu(List<Teacher> teachers, List<Student>? students, List<Course?> courses, object user)
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

            if (!HandleTeacherMenuChoice(choice, teachers, students, courses, user))
            {
                return; 
            }
        }
    }

    private static void DisplayMenuOptions()
    {
        Console.WriteLine("1. Display Teacher Details");
        Console.WriteLine("2. Manage Students");
        Console.WriteLine("3. Manage Courses");
        Console.WriteLine("4. Display Teacher Names");
        Console.WriteLine("5. Display Teachers By Subject");
        Console.WriteLine("6. Display All Teachers");
        Console.WriteLine("7. Add New Teacher");
        Console.WriteLine("8. Remove Teacher");
        Console.WriteLine("9. Exit");
        Console.Write("Enter your choice: ");
    }

    private static bool HandleTeacherMenuChoice(string choice, List<Teacher> teachers, List<Student>? students, List<Course?> courses, object user)
    {
        switch (choice)
        {
            case "1":
                TeacherHandler.DisplayTeacherDetails(teachers, user);
                break;

            case "2":
                SubStudentMenu.DisplayStudentMenu(students, user);
                break;

            case "3":
                SubCourseMenu.DisplayCourseMenu(courses, students, user);
                break;

            case "4":
                TeacherHandler.DisplayTeacherNames(teachers);
                break;

            case "5":
                TeacherHandler.DisplayTeachersBySubject(teachers);
                break;

            case "6":
                TeacherHandler.PromptToDisplayAllTeachers(teachers);
                break;

            case "7":
                TeacherHandler.AddNewTeacher(teachers, user);
                break;

            case "8":
                TeacherHandler.RemoveTeacher(teachers, user);
                break;

            case "9":
                return false; 

            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
        return true; 
    }
}
