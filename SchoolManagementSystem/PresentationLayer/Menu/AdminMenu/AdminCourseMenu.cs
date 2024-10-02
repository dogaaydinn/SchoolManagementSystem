using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu.AdminMenu;

public static class AdminCourseMenu
{
    public static void DisplayCourseMenu(List<Course>? courses, List<Student?>? students, object? user)
    {
        while (true)
        {
            Console.WriteLine("\nAdmin Course Menu:");
            Console.WriteLine("1. Display Course Actions");
            Console.WriteLine("2. Display Course Details");
            Console.WriteLine("3. Display Course Names");
            Console.WriteLine("4. List Students in Courses");
            Console.WriteLine("5. Display Total Courses");
            Console.WriteLine("6. List All Students in Courses");
            Console.WriteLine("7. Display Course Grades");
            Console.WriteLine("8. Enroll Students in Courses");
            Console.WriteLine("9. Remove Student Interactive");
            Console.WriteLine("10. Get Course From User Input");
            Console.WriteLine("11. Get Course By ID");
            Console.WriteLine("12. Get Student From Course");
            Console.WriteLine("13. Display Grades");
            Console.WriteLine("14. Update Course ID");
            Console.WriteLine("15. Update Course Name");
            Console.WriteLine("16. Assign Courses To Students");
            Console.WriteLine("17. Exit");
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
                        CourseHandler.DisplayCourseActions(course, user);
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
                    CourseHandler.DisplayCourseNames(courses, students);
                    break;
                case "4":
                    CourseHandler.ListStudentsInCourses(courses, students);
                    break;
                case "5":
                    CourseHandler.DisplayTotalCourses(courses, students);
                    break;
                case "6":
                    CourseHandler.ListAllStudentsInCourses(courses, students);
                    break;
                case "7":
                    CourseHandler.DisplayCourseGrades(courses, students);
                    break;
                case "8":
                    CourseHandler.EnrollStudentsInCourses(courses, students, user);
                    break;
                case "9":
                    CourseHandler.RemoveStudentInteractive(courses, user);
                    break;
                case "10":
                    CourseHandler.GetCourseFromUserInput(courses);
                    break;
                case "11":
                    CourseHandler.GetCourseById(courses);
                    break;
                case "12":
                    course = SchoolHandler.SelectCourse(courses);
                    if (course != null)
                    {
                        CourseHandler.GetStudentFromCourse(course);
                    }
                    break;
                case "13":
                    course = SchoolHandler.SelectCourse(courses);
                    if (course != null)
                    {
                        CourseHandler.DisplayGrades(course);
                    }
                    break;
                case "14":
                    course = SchoolHandler.SelectCourse(courses);
                    if (course != null)
                    {
                        CourseHandler.UpdateCourseId(course, user as IUser);
                    }
                    break;
                case "15":
                    course = SchoolHandler.SelectCourse(courses);
                    if (course != null)
                    {
                        CourseHandler.UpdateCourseName(course, user as IUser);
                    }
                    break;
                case "16":
                    if (courses != null && students != null)
                    {
                        SchoolHandler.AssignCoursesToStudents(courses, students, user as IUser);
                    }
                    else
                    {
                        Console.WriteLine("Courses or students list is null.");
                    }
                    break;
                case "17":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}