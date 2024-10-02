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
            Console.WriteLine("1. Display Student Details");
            Console.WriteLine("2. Display Student Grades");
            Console.WriteLine("3. Update Student ID");
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
                    var student = SelectStudent(students);
                    if (student != null)
                    {
                        StudentHandler.DisplayStudentDetails(student);
                    }
                    break;
                case "2":
                    student = SelectStudent(students);
                    if (student != null)
                    {
                        StudentHandler.DisplayStudentGrades(student, courses, user);
                    }
                    break;
                case "3":
                    student = SelectStudent(students);
                    if (student != null)
                    {
                        if (user is IUser iUser)
                        {
                            StudentHandler.UpdateStudentId(new List<Student> { student }, iUser);
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

    private static Student? SelectStudent(List<Student?>? students)
    {
        Console.WriteLine("Select a student:");
        if (students != null)
        {
            for (var i = 0; i < students.Count; i++)
            {
                var student = students[i];
                if (student != null)
                {
                    Console.WriteLine($"{i + 1}. {student.GetStudentFullName()} (ID: {student.GetStudentId()})");
                }
            }
        }
        else
        {
            Console.WriteLine("No students available.");
        }

        if (students != null && int.TryParse(Console.ReadLine(), out var studentIndex) && studentIndex >= 1 && studentIndex <= students.Count)
        {
            var selectedStudent = students[studentIndex - 1];
            if (selectedStudent != null)
            {
                return selectedStudent;
            }
        }

        Console.WriteLine("Invalid student selection.");
        return null;
    }

    private static void HandleStudentMenu(string choice, List<Course>? courses, List<Student?>? students, Student? student, object? user)
    {
        switch (choice)
        {
            case "1":
                SchoolHandler.DisplayAllDetails(courses, students, new List<Teacher?>(), student);
                break;
            case "2":
                if (courses == null)
                {
                    Console.WriteLine("Courses list is null. Cannot enroll student in course.");
                }
                else
                {
                    if (students != null)
                    {
                        SchoolHandler.EnrollStudentInCourse(students.ToList(), courses, student);
                    }
                    else
                    {
                        Console.WriteLine("Students list is null. Cannot enroll student in course.");
                    }
                }
                break;
            case "3":
                PersonHandler.DemonstrateActions(student, student);
                break;
            case "4":
                StudentHandler.DisplayStudentActions(student, user);
                break;
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
}