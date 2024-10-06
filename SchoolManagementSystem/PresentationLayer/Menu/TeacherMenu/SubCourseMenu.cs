using SchoolManagementSystem.Interfaces.Actions;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;
using SchoolManagementSystem.PresentationLayer.Handlers.ActionHandler;
using SchoolManagementSystem.PresentationLayer.Helpers;

namespace SchoolManagementSystem.PresentationLayer.Menu.TeacherMenu;

public static class SubCourseMenu
{
    public static void DisplayCourseMenu(List<Course?>? courses, List<Student>? students, object user)
    {
        var nullableStudents = students.Cast<Student?>().ToList();
        var schoolHelper = new SchoolHelper();

        while (true)
        {
            Console.WriteLine("\nCourse Menu:");
            DisplayCourseMenuOptions();
            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                continue;
            }

            if (!HandleCourseMenuChoice(choice, courses, nullableStudents, user, schoolHelper)) return;
        }
    }

    private static void DisplayCourseMenuOptions()
    {
        Console.WriteLine("1. Display Course Actions");
        Console.WriteLine("2. Display Course Details");
        Console.WriteLine("3. Display Course Names");
        Console.WriteLine("4. List Students in Courses");
        Console.WriteLine("5. Enroll Student in Course");
        Console.WriteLine("6. Assign Courses to Students");
        Console.WriteLine("7. Display Total Courses");
        Console.WriteLine("8. Display Course Students");
        Console.WriteLine("9. Get Course From User Input");
        Console.WriteLine("10. Demonstrate Course Methods");
        Console.WriteLine("11. Exit");
        Console.Write("Enter your choice: ");
    }

    private static bool HandleCourseMenuChoice(string choice, List<Course?>? courses, List<Student?> nullableStudents,
        object user, SchoolHelper schoolHelper)
    {
        Course? course;

        switch (choice)
        {
            case "1":
                course = schoolHelper.SelectCourse(courses);
                if (course != null)
                    CourseHandler.DisplayCourseActions(course, user);
                else
                    Console.WriteLine("Error: No course selected.");
                break;
            case "2":
                course = schoolHelper.SelectCourse(courses);
                if (course != null) CourseHandler.DisplayCourseDetails(new List<Course?> { course }, user);
                break;
            case "3":
                CourseHandler.DisplayCourseNames(courses, user);
                break;
            case "4":
                CourseHandler.ListStudentsInCourses(courses, nullableStudents, (IUser)user);
                break;
            case "5":
                SchoolHandler.EnrollStudentInCourse(courses, nullableStudents, (IUser)user);
                break;
            case "6":
                SchoolHandler.AssignCoursesToStudents(schoolHelper,courses, nullableStudents, (IUser)user);
                break;
            case "7":
                schoolHelper.DisplayCourses(courses);
                break;
            case "8":
                if (user != null)
                    CourseHandler.ListStudentsInCourses(courses, nullableStudents, (IUser)user);
                else
                    Console.WriteLine("Error: User is null.");
                break;
            case "9":
                schoolHelper.GetCourseFromUserInput(courses);
                break;
            case "10":
                SchoolActionDemonstrator.DemonstrateCourseActions(new List<ICourseActions>(), courses, (IUser)user);
                break;
            case "11":
                return false;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }

        return true;
    }
}