using System.Globalization;
using SchoolManagementSystem.BusinessLogicLayer.Validations;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Interfaces.Helper;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Helpers;

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

    Student? IStudentHelper.GetStudentById(List<Student> students)
    {
        return GetStudentById(students);
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
        var gpa = InputHelper.GetValidatedDoubleInput("Enter GPA:", 0, 4);

        var studentId = DataProvider.GenerateStudentId();
        var newStudent = new Student(firstName, lastName, dateOfBirth, studentId, gpa);
        newStudent.GeneratePassword(); // Call the inherited method

        students.Add(newStudent);
        Console.WriteLine($"New student added: {newStudent.GetStudentFullName()} (ID: {newStudent.GetStudentId()})");
        Console.WriteLine($"Generated password: {newStudent.GetPassword()}");
    }

    public void RemoveStudent(List<Student?> students, IUser user)
    {
        ValidationHelper.ValidateNotNull(students, "Students list cannot be null.");
        ValidationHelper.ValidateNotNull(user, "User cannot be null.");

        Console.WriteLine("Enter the ID of the student to remove:");
        var studentId = InputHelper.GetValidatedIntInput("Student ID:");

        var studentToRemove = students?.FirstOrDefault(s => s?.GetStudentId() == studentId);
        if (studentToRemove == null)
        {
            Console.WriteLine($"No student found with ID: {studentId}");
            return;
        }

        students?.Remove(studentToRemove);
        Console.WriteLine($"Student ID: {studentId} has been removed successfully.");
    }

    private static void DisplayCourses(Student student)
    {
        Console.WriteLine("Courses:");
        foreach (var course in student.GetEnrolledCourses()) Console.WriteLine(course.GetCourseName());
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

    public static Student? GetStudentById(List<Student> students)
    {
        var studentId = InputHelper.GetValidatedIntInput("Enter the Student ID:");

        if (students == null)
        {
            Console.WriteLine("Students list is null.");
            return null;
        }

        var student = students.FirstOrDefault(s => s.GetStudentId() == studentId);
        if (student == null) Console.WriteLine("Student not found.");

        return student;
    }

    public static void DisplayGrades(Course course)
    {
        var enrolledStudents = course.GetEnrolledStudents();
        if (enrolledStudents == null || !enrolledStudents.Any())
        {
            Console.WriteLine("No students enrolled in this course.");
            return;
        }

        Console.WriteLine($"Grades for {course.GetCourseName()}:");
        foreach (var student in enrolledStudents.OfType<Student>())
            Console.WriteLine(
                $"Student ID: {student.GetStudentId()}, Name: {student.GetStudentFullName()}, Grade: {student.GetAssignedGrades(course)}");
    }

    public static void UpdateStudentFullName(Student student, string newName)
    {
        var names = newName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (names.Length >= 2)
        {
            student.SetFirstName(names[0]);
            student.SetLastName(names[1]);
            Console.WriteLine("Student Name updated successfully.");
        }
        else
        {
            Console.WriteLine("Please enter both first and last names.");
        }
    }
}