using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Helpers;

namespace SchoolManagementSystem.BusinessLogicLayer.Validations;

public static class ValidationHelper
{
    public static void CheckHasPermission(object? user, bool isAdmin = false, bool isTeacherOrAdmin = false)
    {
        if (user == null) throw new UnauthorizedAccessException("User is not authenticated.");

        if (isAdmin && user is not Admin)
            throw new UnauthorizedAccessException("User does not have admin permissions.");

        if (isTeacherOrAdmin && user is not (Teacher or Admin))
            throw new UnauthorizedAccessException("User does not have teacher or admin permissions.");
    }

    public static void ValidateNotNull(object entity, string entityName)
    {
        if (entity == null) throw new ArgumentNullException(entityName, $"{entityName} cannot be null.");
    }

    public static void ValidateUserPermission(object user, bool requireTeacherOrAdmin = false)
    {
        if (user == null) throw new UnauthorizedAccessException("User is not authenticated.");

        if (requireTeacherOrAdmin && user is not (Teacher or Admin))
            throw new UnauthorizedAccessException("User does not have the required teacher or admin permissions.");
    }

    public static void ValidateNotEmpty(string? value, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(errorMessage);
    }

    public static void ValidateAdminAccess(object? user)
    {
        if (user is not Admin) throw new UnauthorizedAccessException("User does not have admin access.");
    }

    public static void ValidateTeacherOrAdminAccess(object user)
    {
        if (user is not (Teacher or Admin))
            throw new UnauthorizedAccessException("User does not have teacher or admin access.");
    }

    public static void ValidateStudentNotNull(Student? student)
    {
        if (student == null) throw new ArgumentNullException(nameof(student), "Student cannot be null.");
    }

    public static void ValidateStudentListNotNull(List<Student?> students)
    {
        if (students == null || students.Count == 0)
            throw new ArgumentNullException(nameof(students), "Student list cannot be null or empty.");
    }

    public static void ValidateUserNotNull(object user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null.");
    }

    public static void ValidateList<T>(List<T> list, string errorMessage)
    {
        if (list == null || list.Count == 0) throw new ArgumentException(errorMessage);
    }

    public static bool ValidateUserInput(string? input, out int validChoice, int min = 1, int max = 11)
    {
        validChoice = 0;
        if (string.IsNullOrEmpty(input) || !int.TryParse(input, out validChoice))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            return false;
        }

        if (validChoice >= min && validChoice <= max) return true;
        Console.WriteLine($"Invalid choice. Please enter a number between {min} and {max}.");
        return false;
    }

    public static void ValidateUserPermissions(object user, bool isAdmin)
    {
        if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null.");

        if (isAdmin && user is not Admin) throw new UnauthorizedAccessException("User does not have admin access.");
    }


    public static Student? SelectAndValidateStudent(List<Student?>? students)
    {
        if (students == null || !students.Any())
        {
            Console.WriteLine("No students available.");
            return null;
        }

        Console.Write("Enter Student ID: ");
        var idInput = Console.ReadLine();
        if (!int.TryParse(idInput, out var id))
        {
            Console.WriteLine("Invalid Student ID.");
            return null;
        }

        var student = students.FirstOrDefault(s => s?.GetStudentId() == id);
        if (student != null) return student;
        Console.WriteLine("Student not found.");
        return null;
    }

    public static bool ValidateStudentList(List<Student?>? students)
    {
        if (students != null && students.Any()) return true;
        Console.WriteLine("Student list cannot be null or empty.");
        return false;
    }

    public static bool ValidateUser(object user)
    {
        if (user is Admin) return true;
        Console.WriteLine("User does not have admin access.");
        return false;
    }

    public static void ValidateUserAccess(object user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null.");

        if (user is not Admin) throw new UnauthorizedAccessException("User does not have admin access.");
    }

    public static void ValidateAdminPermissions(object? user, bool isAdmin = false)
    {
        if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null.");

        if (isAdmin && !(user is Admin)) throw new UnauthorizedAccessException("User does not have admin permissions.");
    }

    public static List<Teacher> ValidateTeacherList(List<Teacher?>? teachers)
    {
        if (teachers == null || !teachers.Any())
            throw new ArgumentException("Teacher list cannot be null or empty.", nameof(teachers));

        return teachers.OfType<Teacher>().ToList();
    }

    public static List<Teacher> ValidateAndFilterTeacherList(List<Teacher?>? teachers)
    {
        if (teachers == null || !teachers.Any())
            throw new ArgumentException("Teacher list cannot be null or empty.", nameof(teachers));

        var nonNullTeachers = teachers.OfType<Teacher>().ToList();
        if (!nonNullTeachers.Any())
            throw new ArgumentException("Teacher list contains no valid teachers.", nameof(teachers));

        return nonNullTeachers;
    }

    public static void Validate(object parameter)
    {
        if (parameter == null) throw new ArgumentNullException(nameof(parameter), "Parameter cannot be null.");
    }

    public static int GetValidatedUserChoice(string prompt, int min, int max)
    {
        while (true)
        {
            var choice = InputHelper.GetValidatedIntInput(prompt);
            if (choice >= min && choice <= max) return choice;
            Console.WriteLine($"Invalid choice. Please select between {min} and {max}.");
        }
    }
}