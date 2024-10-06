using SchoolManagementSystem.BusinessLogicLayer.Validations;
using SchoolManagementSystem.Interfaces.Helper;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Helpers;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class StudentHandler
{
    private static readonly IStudentHelper StudentHelper = new StudentHelper();

    public static void DisplayStudentDetails(Student student)
    {
        if (student is null)
        {
            Console.WriteLine("Student not found.");
            return;
        }

        StudentHelper.DisplayStudentInfo(student);
    }

    public static void UpdateStudentId(List<Student> students, object user)
    {
        ValidationHelper.ValidateUserAccess(user);
        var student = StudentHelper.GetStudentById(students);

        if (student is not null) StudentHelper.UpdateStudentId(student);
    }

    public static void UpdateStudentGpa(Student? student, object user)
    {
        ValidationHelper.ValidateUserAccess(user);
        ValidationHelper.ValidateStudentNotNull(student);

        if (student is not null) StudentHelper.UpdateStudentGpa(student);
    }

    public static void UpdateStudentName(List<Student?>? students, object user)
    {
        ValidationHelper.ValidateUserAccess(user);
        var student = Helpers.StudentHelper.GetStudentById(students);

        if (student is not null)
        {
            Console.Write("Enter new Student Name: ");
            var newName = Console.ReadLine();
            ValidationHelper.ValidateNotEmpty(newName, "New Student Name cannot be empty.");
            Helpers.StudentHelper.UpdateStudentFullName(student, newName);
        }
    }

    public static void AddNewStudent(List<Student> students, IUser user)
    {
        ValidationHelper.ValidateStudentListNotNull(students);
        ValidationHelper.ValidateUserNotNull(user);

        StudentHelper.AddNewStudent(students);
    }

    public static void RemoveStudent(List<Student?> students, Student? student, IUser user)
    {
        ValidationHelper.ValidateStudentListNotNull(students);
        ValidationHelper.ValidateStudentNotNull(student);
        ValidationHelper.ValidateUserNotNull(user);

        if (student is not null) StudentHelper.RemoveStudent(students, student);
    }

    public static void DisplayAllStudents(List<Student?> students)
    {
        if (students is null || !students.Any())
        {
            Console.WriteLine("No students available.");
            return;
        }

        foreach (var student in students.OfType<Student>())
            Console.WriteLine(
                $"Student ID: {student.GetStudentId()}, Name: {student.GetStudentFullName()}, GPA: {student.GetGpa()}");
    }
}