using SchoolManagementSystem.Interfaces.Actions;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;
using SchoolManagementSystem.PresentationLayer.Handlers.ActionHandler;

namespace SchoolManagementSystem.PresentationLayer.Menu.AdminMenu;

public static class CourseMenu
{
    public static void DisplayCourseMenu(List<Course> courses, List<Student>? students, IUser user)
    {
        var schoolHelper = new SchoolHelper(); 

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

            if (!HandleCourseMenuChoice(choice, courses, students, user, schoolHelper))
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

    private static bool HandleCourseMenuChoice(string choice, List<Course> courses, List<Student?> nullableStudents, IUser user, SchoolHelper schoolHelper)
    {
        Course? course = null;

        switch (choice)
        {
            case "1":
                course = schoolHelper.SelectCourse(courses); 
                if (course != null)
                {
                    CourseHandler.DisplayCourseActions(course, user);
                }
                else
                {
                    Console.WriteLine("Error: No course selected.");
                }
                break;
            case "2":
                course = schoolHelper.SelectCourse(courses); 
                if (course != null)
                {
                    CourseHandler.DisplayCourseDetails(new List<Course?> { course }, user);
                }
                break;
            case "3":
                CourseHandler.DisplayCourseNames(courses, user);
                break;
            case "4":
                CourseHandler.ListStudentsInCourses(courses, nullableStudents, user);
                break;
            case "5":
                SchoolHandler.EnrollStudentInCourse(nullableStudents, courses, user);
                break;
            case "6":
                SchoolHandler.AssignCoursesToStudents(courses, nullableStudents, user);
                break;
            case "7":
                schoolHelper.DisplayCourses(courses); 
                break;
            case "8":
                CourseHandler.ListStudentsInCourses(courses, nullableStudents, user);
                break;
            case "9":
                schoolHelper.GetCourseFromUserInput(courses); 
                break;
            case "10":
                var courseActionsList = courses.Cast<ICourseActions>().ToList();
                SchoolActionDemonstrator.DemonstrateCourseActions(courseActionsList, courses, user);
                break;
            case "11":
                CourseHandler.ListStudentsInCourses(courses, nullableStudents, user);
                break;
            case "12":
                return false;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }

        return true;
    }
}