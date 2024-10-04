using SchoolManagementSystem.BusinessLogicLayer.Validations;
using SchoolManagementSystem.Interfaces.Helper;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Helpers;

public class TeacherHelper : ITeacherHelper
{
    private static void ValidateUserPermissions(object user)
    {
        ValidationHelper.ValidateNotNull(user, "User cannot be null.");
        ValidationHelper.ValidateUserPermissions(user, isAdmin: true);
    }

    public static void AddNewTeacher(List<Teacher?> teachers, object user)
    {
        ValidateUserPermissions(user);
        var nonNullTeachers = ValidateTeacherList(teachers);

        var id = GetValidatedTeacherId(nonNullTeachers);
        if (!id.HasValue) return;

        var names = GetValidatedTeacherName();
        if (names == null) return;

        var subject = GetValidatedTeacherSubject();
        if (string.IsNullOrEmpty(subject)) return;

        var newTeacher = new Teacher(id.Value.ToString(), names[0], DateTime.Now, 0, subject);
        nonNullTeachers.Add(newTeacher);
        Console.WriteLine("Teacher added successfully.");
    }

    private static int? GetValidatedTeacherId(List<Teacher> nonNullTeachers)
    {
        Console.Write("Enter Teacher ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id) || 
            nonNullTeachers.Any(t => t.GetTeacherId() == id))
        {
            Console.WriteLine("Invalid or duplicate Teacher ID.");
            return null;
        }
        return id;
    }

    private static string[]? GetValidatedTeacherName()
    {
        Console.Write("Enter Teacher Name (First Last): ");
        var name = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(name, "Teacher Name cannot be empty.");
        var names = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (names.Length != 2)
        {
            Console.WriteLine("Please enter both first and last names.");
            return null;
        }
        return names;
    }

    private static string GetValidatedTeacherSubject()
    {
        Console.Write("Enter Teacher Subject: ");
        var subject = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(subject, "Teacher Subject cannot be empty.");
        return subject;
    }

    private static List<Teacher> ValidateTeacherList(List<Teacher?> teachers)
    {
        var nonNullTeachers = teachers?.OfType<Teacher>().ToList();
        ValidationHelper.ValidateList(nonNullTeachers, "Teacher list cannot be null or empty.");
        return nonNullTeachers!;
    }

    public void DisplayMenuOptions(string[] options)
    {
        ValidationHelper.ValidateNotNull(options, "Options cannot be null.");
        for (var i = 0; i < options.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {options[i]}");
        }
        Console.Write("Enter your choice: ");
    }

    public int GetValidatedUserChoice(int maxOptions)
    {
        if (maxOptions <= 0) throw new ArgumentOutOfRangeException(nameof(maxOptions), "Max options must be greater than 0.");

        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > maxOptions)
        {
            Console.WriteLine($"Invalid choice. Please select a number between 1 and {maxOptions}.");
        }
        return choice;
    }

    public void DisplayTeacherDetails(List<Teacher?> teachers, object user)
    {
        ValidateUserPermissions(user);
        var nonNullTeachers = ValidateTeacherList(teachers);

        Console.WriteLine("Choose an option to search by:");
        DisplayMenuOptions(new[] { "Teacher ID", "Teacher Name", "List all teachers", "Teachers by subject" });

        var choice = GetValidatedUserChoice(4);
        switch (choice)
        {
            case 1:
                DisplayTeacherById(nonNullTeachers);
                break;
            case 2:
                DisplayTeacherByName(nonNullTeachers);
                break;
            case 3:
                DisplayAllTeachers(nonNullTeachers);
                break;
            case 4:
                DisplayTeachersBySubject(nonNullTeachers);
                break;
        }
    }

    private void DisplayTeacherById(List<Teacher> nonNullTeachers)
    {
        var teacher = GetTeacherById(nonNullTeachers);
        DisplayTeacherDetails(teacher);
    }

    private void DisplayTeacherByName(List<Teacher> nonNullTeachers)
    {
        var teacher = GetTeacherByName(nonNullTeachers);
        DisplayTeacherDetails(teacher);
    }

    private static void DisplayTeacherDetails(Teacher teacher)
    {
        if (teacher != null)
        {
            Console.WriteLine($"Teacher ID: {teacher.GetTeacherId()}, Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
        }
        else
        {
            Console.WriteLine("Teacher not found.");
        }
    }

    public void UpdateTeacherDetails(List<Teacher?> teachers, object user)
    {
        ValidateUserPermissions(user);
        var nonNullTeachers = ValidateTeacherList(teachers);

        while (true)
        {
            Console.WriteLine("\nTeacher Update Menu:");
            DisplayMenuOptions(new[] { "Update Teacher ID", "Update Teacher Subject", "Update Teacher Name", "Exit" });

            var choice = GetValidatedUserChoice(4);
            switch (choice)
            {
                case 1:
                    UpdateTeacherId(nonNullTeachers);
                    break;
                case 2:
                    UpdateTeacherSubject(nonNullTeachers);
                    break;
                case 3:
                    UpdateTeacherName(nonNullTeachers);
                    break;
                case 4:
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please select a valid option.");
                    break;
            }
        }
    }

    private static Teacher GetTeacherById(List<Teacher>? teachers)
    {
        Console.Write("Enter Teacher ID: ");
        var id = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(id, "Teacher ID cannot be empty.");
        var teacher = teachers?.FirstOrDefault(t => t.GetTeacherId().ToString() == id);
        ValidationHelper.ValidateNotNull(teacher, "Teacher not found.");
        return teacher;
    }

    private static Teacher GetTeacherByName(List<Teacher>? teachers)
    {
        Console.Write("Enter Teacher Name: ");
        var name = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(name, "Teacher Name cannot be empty.");
        var teacher = teachers?.FirstOrDefault(t => t.GetTeacherFullName().Equals(name, StringComparison.OrdinalIgnoreCase));
        ValidationHelper.ValidateNotNull(teacher, "Teacher not found.");
        return teacher;
    }

    public static void DisplayTeachersBySubject(List<Teacher?> teachers)
    {
        Console.Write("Enter Subject Name: ");
        var subject = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(subject, "Subject cannot be empty.");
        var filteredTeachers = teachers?.Where(t => t.GetSubject().Equals(subject, StringComparison.OrdinalIgnoreCase)).ToList();
        ValidationHelper.ValidateNotNull(filteredTeachers, "No teachers found for this subject.");
        DisplayTeacherNames(filteredTeachers);
    }

    private static void UpdateTeacherId(List<Teacher>? teachers)
    {
        var teacher = GetTeacherById(teachers);
        if (teacher == null) return;

        Console.Write("Enter new Teacher ID: ");
        var newId = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(newId, "New Teacher ID cannot be empty.");
        teacher.SetTeacherId(int.Parse(newId));
        Console.WriteLine("Teacher ID updated successfully.");
    }

    private static void UpdateTeacherSubject(List<Teacher>? teachers)
    {
        var teacher = GetTeacherById(teachers);
        if (teacher == null) return;

        Console.Write("Enter new Subject: ");
        var newSubject = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(newSubject, "New Subject cannot be empty.");
        teacher.SetSubject(newSubject);
        Console.WriteLine("Teacher subject updated successfully.");
    }

    private static void UpdateTeacherName(List<Teacher>? teachers)
    {
        var teacher = GetTeacherById(teachers);
        if (teacher == null) return;

        var names = GetValidatedTeacherName();
        if (names == null) return;

        teacher.SetFirstName(names[0]);
        teacher.SetLastName(names[1]);
        Console.WriteLine("Teacher name updated successfully.");
    }

    private static void DisplayAllTeachers(List<Teacher?> teachers)
    {
        Console.WriteLine("All Teachers:");
        foreach (var teacher in teachers)
        {
            if (teacher != null)
            {
                Console.WriteLine($"Teacher ID: {teacher.GetTeacherId()}, Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
            }
        }
    }

    private static void DisplayTeacherNames(List<Teacher?> teachers)
    {
        Console.WriteLine("Filtered Teachers:");
        foreach (var teacher in teachers)
        {
            if (teacher != null)
            {
                Console.WriteLine($"Teacher ID: {teacher.GetTeacherId()}, Name: {teacher.GetTeacherFullName()}");
            }
        }
    }
}
