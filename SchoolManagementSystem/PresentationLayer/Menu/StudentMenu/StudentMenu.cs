using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu.StudentMenu;

public static class StudentMenu
{
    public static void DisplayStudentMenu(List<Student?>? students, List<Course>? courses, object? user)
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

            if (!HandleMenuChoice(choice, students, courses, user))
            {
                return; 
            }
        }
    }

    private static void DisplayMenuOptions()
    {
        Console.WriteLine("1. Display Student Details");
        Console.WriteLine("2. Display Student Grades");
        Console.WriteLine("3. Update Student ID");
        Console.WriteLine("4. Exit");
        Console.Write("Enter your choice: ");
    }

    private static bool HandleMenuChoice(string choice, List<Student?>? students, List<Course>? courses, object? user)
    {
        var student = SchoolHelper.SelectStudent(students);
        if (student == null) return true; 

        switch (choice)
        {
            case "1":
                StudentHandler.DisplayStudentDetails(student);
                break;
            case "2":
                StudentHandler.DisplayStudentGrades(student, courses, user);
                break;
            case "3":
                if (user is IUser iUser)
                {
                    StudentHandler.UpdateStudentId(new List<Student> { student }, iUser);
                }
                else
                {
                    Console.WriteLine("Invalid user type. Operation not permitted.");
                }
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
