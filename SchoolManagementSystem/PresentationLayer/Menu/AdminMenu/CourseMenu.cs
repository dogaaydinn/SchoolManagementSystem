using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu.AdminMenu;

public static class CourseMenu
{
    public static void DisplayCourseMenu(List<Course>? courses, List<Student?> students, object? user)
    {
        while (true)
        {
            Console.WriteLine("\nCourse Menu:");
            Console.WriteLine("1. Display Course Details");
            Console.WriteLine("2. Update Course ID");
            Console.WriteLine("3. Update Course Name");
            Console.WriteLine("4. Exit");
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
                    var course = SchoolHandler.SelectCourse(courses);
                    if (course != null)
                    {
                        CourseHandler.DisplayCourseDetails(new List<Course> { course }, user);
                    }
                    break;
                case "2":
                    course = SchoolHandler.SelectCourse(courses);
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
                    break;
                case "3":
                    course = SchoolHandler.SelectCourse(courses);
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
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}