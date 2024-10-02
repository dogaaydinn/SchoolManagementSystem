using System.Globalization;
using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.Interfaces.User;
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
    public static void DisplayStudentGrades(Student? student, List<Course>? courses, object? user)
    {
        Exceptions.CheckHasPermissionToViewGrades(user, student);

        if (student == null || courses == null) return;
        Console.WriteLine($"Grades for {student.GetStudentFullName()}:");
        foreach (var course in courses)
        {
            var grade = course.GetAssignedGrades(student);
            Console.WriteLine($"{course.GetCourseName()}: {grade.ToString(CultureInfo.InvariantCulture)}");
        }
    }
    public static void UpdateStudentId(List<Student>? students, object? user)
    {
        Exceptions.CheckHasPermission(user, isAdmin: true);

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
    public static void DisplayStudentActions(Student? student, object? user)
    {
        Exceptions.CheckStudentNotNull(student);
        if (student == null) return;

        Exceptions.CheckHasPermission(user, student);

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

    private static void DisplayStudentGrades(Student? student, object? user)
    {
        Exceptions.CheckHasPermissionToViewGrades(user, student);

        if (student == null) return;
        Console.WriteLine($"Grades for {student.GetStudentFullName()}:");
        foreach (var course in student.GetEnrolledCourses())
        {
            var grade = student.GetAssignedGrades(course);
            Console.WriteLine($"Course: {course.GetCourseName()}, Grade: {grade}");
        }
    }
    public static void UpdateStudentGpa(Student? student, object? user)
    {
        Exceptions.CheckHasPermission(user, isTeacherOrAdmin: true);
        Exceptions.CheckStudentNotNull(student);

        if (student != null)
        {
            Console.WriteLine($"Current GPA: {student.GetGpa()}");
            Console.WriteLine("Enter new GPA (or type 'exit' to cancel):");
        }
        var input = Console.ReadLine()?.Trim().ToLower();
        if (input == "exit") return;

        if (student == null) return;
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
    public static Student? GetStudentById(List<Student>? students)
    {
        Console.WriteLine("Enter the Student ID:");
        if (!int.TryParse(Console.ReadLine(), out var studentId))
        {
            Console.WriteLine("Invalid Student ID.");
            return null;
        }

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
    public static void UpdateStudentName(Student? student, IUser user)
    {
        if (student == null)
        {
            throw new Exceptions.NullStudentException("Student is null.");
        }

        if (user == null)
        {
            throw new Exceptions.NullUserException("User is null.");
        }

        Console.Write("Enter new student name: ");
        var newName = Console.ReadLine();

        if (string.IsNullOrEmpty(newName))
        {
            throw new Exceptions.InvalidNameException("Name cannot be empty.");
        }

        student.SetStudentName(newName);
        Console.WriteLine("Student name updated successfully.");
    }
    public static void AddNewStudent(List<Student>? students, IUser? user)
    {
        if (students == null)
        {
            throw new Exceptions.NullStudentsListException("Students list is null.");
        }

        if (user == null)
        {
            throw new Exceptions.NullUserException("User is null.");
        }

        Console.Write("Enter first name: ");
        var firstName = Console.ReadLine();

        Console.Write("Enter last name: ");
        var lastName = Console.ReadLine();

        Console.Write("Enter date of birth (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out var dateOfBirth))
        {
            Console.WriteLine("Invalid date of birth.");
            return;
        }

        Console.Write("Enter student ID: ");
        if (!int.TryParse(Console.ReadLine(), out var studentId))
        {
            Console.WriteLine("Invalid student ID.");
            return;
        }

        Console.Write("Enter GPA: ");
        if (!double.TryParse(Console.ReadLine(), out var gpa))
        {
            Console.WriteLine("Invalid GPA.");
            return;
        }

        Console.Write("Enter student name: ");
        var name = Console.ReadLine();

        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Invalid input. All fields are required.");
            return;
        }

        var newStudent = new Student(firstName, lastName, dateOfBirth, studentId, gpa, name);

        students.Add(newStudent);
        Console.WriteLine("New student added successfully.");
    }
    public static void RemoveStudent(List<Student?>? students, Student? student, IUser? user)
    {
        if (students == null)
        {
            throw new Exceptions.NullStudentsListException("Students list is null.");
        }

        if (student == null)
        {
            throw new Exceptions.NullStudentException("Student is null.");
        }

        if (user == null)
        {
            throw new Exceptions.NullUserException("User is null.");
        }

        Console.WriteLine(students.Remove(student)
            ? "Student removed successfully."
            : "Error: Student could not be removed.");
    }
    public static void DisplayAllStudents(List<Student?>? students)
    {
        if (students == null || students.Count == 0)
        {
            Console.WriteLine("No students available.");
            return;
        }

        foreach (var student in students)
        {
            Console.WriteLine($"Student ID: {student?.GetStudentId()}, Name: {student?.GetStudentFullName()}, GPA: {student?.GetGpa()}");
        }
    }
}