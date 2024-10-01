using System.Globalization;
using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class StudentHandler
{
    public static void DisplayStudentDetails(Student student)
    {
        Console.WriteLine($"Student ID: {student.GetStudentId()}");
        Console.WriteLine($"Name: {student.GetStudentFullName()}");
        Console.WriteLine($"GPA: {student.GetGpa()}");

        Console.WriteLine("Courses:");
        foreach (var course in student.GetEnrolledCourses())
        {
            Console.WriteLine(course.GetCourseName());
        }

        Console.WriteLine("Grades:");
        foreach (var course in student.GetEnrolledCourses())
        {
            var grade = student.GetAssignedGrades(course);
            Console.WriteLine(grade.ToString());
        }
    }
    
    public static void DisplayStudentGrades(Student student, List<Course> courses, object user)
    {
        Exceptions.Expectations.CheckHasPermissionToViewGrades(user, student);

        Console.WriteLine($"Grades for {student.GetStudentFullName()}:");
        foreach (var course in courses)
        {
            var grade = course.GetAssignedGrades(student);
            Console.WriteLine($"{course.GetCourseName()}: {grade.ToString(CultureInfo.InvariantCulture)}");
        }
    }


    public static void UpdateStudentId(List<Student> students, object user)
    {
        Exceptions.Expectations.CheckHasPermission(user, isAdmin: true);

        var student = GetStudentById(students);
        if (student == null) return;

        Console.WriteLine("Enter the new Student ID:");
        if (!int.TryParse(Console.ReadLine(), out var newStudentId))
        {
            Console.WriteLine("Invalid new Student ID.");
            return;
        }

        student.UpdateStudentId(newStudentId);
        Console.WriteLine($"Student ID updated to {newStudentId} for {student.GetStudentFullName()}.");
    }

    public static void DisplayStudentActions(Student? student, object user)
    {
        Exceptions.Expectations.CheckStudentNotNull(student);
        if (student == null) return;

        Exceptions.Expectations.CheckHasPermission(user, student);

        Console.WriteLine($"Actions for {student.GetStudentFullName()} (ID: {student.GetStudentId()}):");
        Console.WriteLine("1. View grades");
        Console.WriteLine("2. Update GPA");
        Console.WriteLine("3. Exit");
        Console.Write("Enter your choice: ");

        if (int.TryParse(Console.ReadLine(), out var choice))
        {
            switch (choice)
            {
                case 1:
                    DisplayStudentGrades(student, user);
                    break;
                case 2:
                    UpdateStudentGpa(student, user);
                    break;
                case 3:
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please try again.");
        }
    }

    private static void DisplayStudentGrades(Student student, object user)
    {
        Exceptions.Expectations.CheckHasPermissionToViewGrades(user, student);

        Console.WriteLine($"Grades for {student.GetStudentFullName()}:");
        foreach (var course in student.GetEnrolledCourses())
        {
            var grade = student.GetAssignedGrades(course);
            Console.WriteLine($"Course: {course.GetCourseName()}, Grade: {grade}");
        }
    }

    private static void UpdateStudentGpa(Student student, object user)
    {
        Exceptions.Expectations.CheckHasPermission(user, isTeacherOrAdmin: true);
        Exceptions.Expectations.CheckStudentNotNull(student);

        Console.WriteLine($"Current GPA: {student.GetGpa()}");
        Console.WriteLine("Enter new GPA (or type 'exit' to cancel):");

        var input = Console.ReadLine()?.Trim().ToLower();
        if (input == "exit") return;

        if (double.TryParse(input, out var newGpa) && newGpa is >= 0 and <= 4)
        {
            student.SetGpa(newGpa);
            Console.WriteLine($"Updated GPA for {student.GetStudentFullName()} to: {newGpa}");
        }
        else
        {
            Console.WriteLine("Invalid GPA. Please enter a value between 0 and 4.");
        }
    }

    private static Student? GetStudentById(List<Student> students)
    {
        Console.WriteLine("Enter the Student ID:");
        if (!int.TryParse(Console.ReadLine(), out var studentId))
        {
            Console.WriteLine("Invalid Student ID.");
            return null;
        }

        var student = students.FirstOrDefault(s => s.GetStudentId() == studentId);
        if (student == null)
        {
            Console.WriteLine("Student not found.");
        }

        return student;
    }
}