using System.Globalization;
using SchoolManagementSystem.Interfaces.Helper;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer;

public class StudentHelper : IStudentHelper
{
    public void DisplayStudentInfo(Student student)
    {
        Console.WriteLine($"Student ID: {student.GetStudentId()}");
        Console.WriteLine($"Name: {student.GetStudentFullName()}");
        Console.WriteLine($"GPA: {student.GetGpa()}");
        
        DisplayCourses(student);
        DisplayGrades(student);
    }

    private static void DisplayCourses(Student student)
    {
        Console.WriteLine("Courses:");
        foreach (var course in student.GetEnrolledCourses())
        {
            Console.WriteLine(course.GetCourseName());
        }
    }

    public static void DisplayGrades(Student student)
    {
        Console.WriteLine("Grades:");
        foreach (var course in student.GetEnrolledCourses())
        {
            var grade = student.GetAssignedGrades(course);
            Console.WriteLine($"{course.GetCourseName()}: {grade.ToString(CultureInfo.InvariantCulture)}");
        }
    }

    public static Student? GetStudentById(List<Student>? students)
    {
        var studentId = InputHelper.GetValidatedIntInput("Enter the Student ID:");

        if (students == null)
        {
            Console.WriteLine("Students list is null.");
            return null;
        }

        var student = students.FirstOrDefault(s => s.GetStudentId() == studentId);
        if (student == null)
        {
            Console.WriteLine("Student not found.");
        }

        return student;
    }

    public void UpdateStudentId(Student student)
    {
        var newStudentId = InputHelper.GetValidatedIntInput("Enter the new Student ID:");
        student.UpdateStudentId(newStudentId);
        Console.WriteLine($"Student ID updated to {newStudentId} for {student.GetStudentFullName()}.");
    }

    public void UpdateStudentGpa(Student student)
    {
        Console.WriteLine($"Current GPA: {student.GetGpa()}");
        var newGpa = InputHelper.GetValidatedDoubleInput("Enter new GPA (or type 'exit' to cancel):", 0, 4);

        student.SetGpa(newGpa);
        Console.WriteLine($"Updated GPA for {student.GetStudentFullName()} to: {newGpa}");
    }
    
    public void AddNewStudent(List<Student> students)
    {
        var firstName = InputHelper.GetValidatedStringInput("Enter first name:");
        var lastName = InputHelper.GetValidatedStringInput("Enter last name:");
        var dateOfBirth = InputHelper.GetValidatedDateInput("Enter date of birth (yyyy-MM-dd):");
        var studentId = InputHelper.GetValidatedIntInput("Enter student ID:");
        var gpa = InputHelper.GetValidatedDoubleInput("Enter GPA:", 0, 4);

        var newStudent = new Student(firstName, lastName, dateOfBirth, studentId, gpa);
        students.Add(newStudent);

        Console.WriteLine("New student added successfully.");
    }

    public static void RemoveStudent(List<Student> students, Student student)
    {
        Console.WriteLine(students.Remove(student)
            ? "Student removed successfully."
            : "Error: Student could not be removed.");
    }
}
