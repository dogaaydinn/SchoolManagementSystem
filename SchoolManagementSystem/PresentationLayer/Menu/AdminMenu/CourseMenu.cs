using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu.AdminMenu;

public static class CourseMenu
{
    public static void DisplayCourseMenu(List<Course>? courses, List<Student>? students, IUser? user)
    {
        while (true)
        {
            Console.WriteLine("\nCourse Menu:");
            DisplayMenuOptions();

            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                continue;
            }

            if (!HandleMenuChoice(choice, courses, user))
            {
                return; 
            }
        }
    }

    private static void DisplayMenuOptions()
    {
        Console.WriteLine("1. Display Course Details");
        Console.WriteLine("2. Update Course ID");
        Console.WriteLine("3. Update Course Name");
        Console.WriteLine("4. Exit");
        Console.Write("Enter your choice: ");
    }

    private static bool HandleMenuChoice(string choice, List<Course>? courses, IUser? user)
    {
        Course? selectedCourse = null;

        switch (choice)
        {
            case "1":
                selectedCourse = SchoolHandler.SelectCourse(courses);
                if (selectedCourse != null)
                {
                    CourseHandler.DisplayCourseDetails(new List<Course> { selectedCourse }, user);
                }
                break;

            case "2":
                selectedCourse = SchoolHandler.SelectCourse(courses);
                UpdateCourseId(selectedCourse, user);
                break;

            case "3":
                selectedCourse = SchoolHandler.SelectCourse(courses);
                UpdateCourseName(selectedCourse, user);
                break;

            case "4":
                return false; // Exit option selected
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }

        return true; // Continue the menu loop
    }

    private static void UpdateCourseId(Course? course, IUser? user)
    {
        if (course != null)
        {
            if (user is IUser iUser)
            {
                CourseHandler.UpdateCourseId(course, iUser);
            }
            else
            {
                Console.WriteLine("Invalid user type. Operation not permitted.");
            }
        }
    }

    private static void UpdateCourseName(Course? course, IUser? user)
    {
        if (course != null)
        {
            if (user is IUser iUser)
            {
                CourseHandler.UpdateCourseName(course, iUser);
            }
            else
            {
                Console.WriteLine("Invalid user type. Operation not permitted.");
            }
        }
    }
}
