using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer.Menu;

public static class StudentMenu
{
    public static void DisplayStudentMenu(List<Student> students, List<Course> courses)
    {
        while (true)
        {
            Console.WriteLine("\nStudent Menu:");
            Console.WriteLine("1. Display All Details");
            Console.WriteLine("2. Display Student Details");
            Console.WriteLine("3. Display Course Details");
            Console.WriteLine("4. Demonstrate Actions");
            Console.WriteLine("5. Display Student Names");
            Console.WriteLine("6. Demonstrate Student Actions");
            Console.WriteLine("7. Display Course Names");
            Console.WriteLine("8. Update Student ID");
            Console.WriteLine("9. Exit");
            Console.Write("Enter your choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    StudentHandler.DisplayAllDetails(students, courses);
                    break;
                case "2":
                    StudentHandler.DisplayStudentDetails(students);
                    break;
                case "3":
                    StudentHandler.DisplayCourseDetails(courses);
                    break;
                case "4":
                    if (students.Count > 0)
                    {
                        PersonHandler.DemonstrateActions(students[0]);
                    }
                    else
                    {
                        Console.WriteLine("No students available to demonstrate actions.");
                    }
                    break;
                case "5":
                    StudentHandler.DisplayStudentNames(students);
                    break;
                case "6":
                    StudentHandler.DemonstrateStudentActions(students, courses);
                    break;
                case "7":
                    StudentHandler.DisplayCourseNames(courses);
                    break;
                case "8":
                    StudentHandler.UpdateStudentId(students);
                    break;
                case "9":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}