using SchoolManagementSystem.BusinessLogicLayer.Validations;
using SchoolManagementSystem.Interfaces.Helper;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class StudentHandler
{
    private static readonly IStudentHelper StudentHelper = new StudentHelper();
    
    public static void DisplayStudentDetails(Student student)
    {
        if (student == null) 
        {
            Console.WriteLine("Student not found.");
            return;
        }
        
        StudentHelper.DisplayStudentInfo(student);
    }
    
    public static void UpdateStudentId(List<Student>? students, object? user)
    {
        ValidationHelper.ValidateAdminAccess(user);
        
        var student = StudentHelper.GetStudentById(students);
        if (student != null)
        {
            StudentHelper.UpdateStudentId(student);
        }
    }

    public static void UpdateStudentGpa(Student? student, object? user)
    {
        ValidationHelper.ValidateTeacherOrAdminAccess(user);
        ValidationHelper.ValidateStudentNotNull(student);

        if (student != null)
        {
            StudentHelper.UpdateStudentGpa(student);
        }
    }
    
    public static void UpdateStudentName(List<Student?>? students, object? user)
    {
        ValidationHelper.ValidateUser(user);
        var nonNullStudents = students?.OfType<Student>().ToList();
        ValidationHelper.ValidateList(nonNullStudents, "Student list cannot be null or empty.");

        Console.Write("Enter Student ID: ");
        var idInput = Console.ReadLine();
        if (!int.TryParse(idInput, out int id))
        {
            Console.WriteLine("Invalid Student ID.");
            return;
        }

        var student = nonNullStudents.FirstOrDefault(s => s.GetStudentId() == id);
        if (student == null)
        {
            Console.WriteLine("Student not found.");
            return;
        }

        Console.Write("Enter new Student Name: ");
        var newName = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(newName, "New Student Name cannot be empty.");
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
    
    public static void AddNewStudent(List<Student>? students, IUser? user)
    {
        ValidationHelper.ValidateStudentListNotNull(students);
        ValidationHelper.ValidateUserNotNull(user);

        StudentHelper.AddNewStudent(students);
    }
    
    public static void RemoveStudent(List<Student>? students, Student? student, IUser? user)
    {
        ValidationHelper.ValidateStudentListNotNull(students);
        ValidationHelper.ValidateStudentNotNull(student);
        ValidationHelper.ValidateUserNotNull(user);

        if (student != null)
        {
            StudentHelper.RemoveStudent(students, student);
        }
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
            if (student != null)
            {
                Console.WriteLine($"Student ID: {student.GetStudentId()}, Name: {student.GetStudentFullName()}, GPA: {student.GetGpa()}");
            }
        }
    }
}
