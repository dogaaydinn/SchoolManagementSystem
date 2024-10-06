using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;
using SchoolManagementSystem.PresentationLayer.Helpers;

namespace SchoolManagementSystem.PresentationLayer.Menu.StudentMenu;

public static class StudentMenu
{
    public static void DisplayStudentMenu(List<Student?> students, List<Course> courses, object user)
    {
        while (true)
        {
            Console.WriteLine("\nStudent Menu:");
            DisplayMenuOptions();

            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                continue;
            }

            if (!HandleMenuChoice(choice, students, courses, user)) return;
        }
    }

    private static void DisplayMenuOptions()
    {
        Console.WriteLine("1. Display Student Details");
        Console.WriteLine("2. Display Student Grades");
        Console.WriteLine("3. Display Course Details");
        Console.WriteLine("4. Display Total Courses");
        Console.WriteLine("5. Update Student ID");
        Console.WriteLine("6. Exit");
        Console.Write("Enter your choice: ");
    }

    private static bool HandleMenuChoice(string choice, List<Student?> students, List<Course> courses, object user)
    {
        var schoolHelper = new SchoolHelper();
        var student = schoolHelper.SelectStudent(students);
        var course = schoolHelper.SelectCourse(courses);

        switch (choice)
        {
            case "1":
                if (student != null)
                    StudentHandler.DisplayStudentDetails(student);
                else
                    Console.WriteLine("Error: No student selected.");
                break;
            case "2":
                if (course != null)
                    StudentHelper.DisplayGrades(course);
                else
                    Console.WriteLine("Error: No course selected.");
                break;
            case "3":
                if (course != null)
                    CourseHandler.DisplayCourseDetails(new List<Course?> { course }, user);
                else
                    Console.WriteLine("Error: No course selected.");
                break;
            case "4":
                CourseHandler.DisplayTotalCourses(courses);
                break;
            case "5":
                if (student != null)
                    StudentHandler.UpdateStudentId(new List<Student> { student }, user);
                else
                    Console.WriteLine("Error: No student selected.");
                break;
            case "6":
                return false;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }

        return true;
    }
}