using SchoolManagementSystem.Models;
using SchoolManagementSystem.PresentationLayer.Handlers;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Menu.TeacherMenu;

public class SubCourseMenu
{
    public static void DisplayCourseMenu(List<Course> courses, List<Student> students, object? user)
    {
        var nullableStudents = students.Cast<Student?>().ToList();
        while (true)
        {
            Console.WriteLine("\nCourse Menu:");
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

            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                continue;
            }

            Course? course = null;

            switch (choice)
            {
                case "1":
                    var selectedCourse = SchoolHandler.SelectCourse(courses);
                    if (selectedCourse != null)
                    {
                        CourseHandler.DisplayCourseActions(selectedCourse, user);
                    }
                    else
                    {
                        Console.WriteLine("Error: No course selected.");
                    }
                    break;
                case "2":
                    course = SchoolHandler.SelectCourse(courses);
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
                    // Implement Demonstrate Course Methods logic
                    break;
                case "11":
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
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}