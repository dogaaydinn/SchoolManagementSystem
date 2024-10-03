using SchoolManagementSystem.PresentationLayer.Handlers;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Menu.TeacherMenu;

public static class SubCourseMenu
{
    public static void DisplayCourseMenu(List<Course> courses, List<Student>? students, object? user)
    {
        var nullableStudents = students.Cast<Student?>().ToList();

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

            if (!HandleCourseMenuChoice(choice, courses, nullableStudents, user))
            {
                return; 
            }
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
        Console.WriteLine("11. Get Student From Course");
        Console.WriteLine("12. Exit");
        Console.Write("Enter your choice: ");
    }

    private static bool HandleCourseMenuChoice(string choice, List<Course> courses, List<Student?> nullableStudents, object? user)
    {
        Course? course = null;

        switch (choice)
        {
            case "1":
                course = SchoolHelper.SelectCourse(courses);
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
                course = SchoolHelper.SelectCourse(courses);
                if (course != null)
                {
                    CourseHandler.DisplayCourseDetails(new List<Course> { course }, user);
                }
                break;

            case "3":
                CourseHandler.DisplayCourseNames(courses, user);
                break;

            case "4":
                CourseHandler.ListStudentsInCourses(courses, user);
                break;

            case "5":
                CourseHandler.EnrollStudentsInCourses(courses, nullableStudents, user);
                break;

            case "6":
                SchoolHandler.AssignCoursesToStudents(courses, nullableStudents, user);
                break;

            case "7":
                CourseHandler.DisplayTotalCourses(courses, user);
                break;

            case "8":
                CourseHandler.ListStudentsInCourses(courses, user);
                break;

            case "9":
                CourseHandler.GetCourseFromUserInput(courses);
                break;

            case "10":
                CourseHandler.DemonstrateCourseMethods(courses, user);
                break;

            case "11":
                course = SchoolHelper.SelectCourse(courses);
                if (course != null)
                {
                    CourseHandler.GetStudentFromCourse(course);
                }
                else
                {
                    Console.WriteLine("Error: No course selected.");
                }
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
