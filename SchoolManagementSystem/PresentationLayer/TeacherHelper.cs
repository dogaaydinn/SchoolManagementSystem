using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.BusinessLogicLayer.Validations;
using SchoolManagementSystem.Interfaces.Helper;

namespace SchoolManagementSystem.PresentationLayer;

public class TeacherHelper : ITeacherHelper
{
    private static void ValidateUserPermissions(object? user)
    {
        ValidationHelper.ValidateNotNull(user, "User cannot be null.");
        ValidationHelper.ValidateUserPermissions(user, isAdmin: true);
    }

    public static void AddNewTeacher(List<Teacher?>? teachers, object? user)
    {
        ValidateUserPermissions(user);
        var nonNullTeachers = ValidateTeacherList(teachers);

        var id = GetValidatedTeacherId(nonNullTeachers);
        if (id == null) return;

        var names = GetValidatedTeacherName();
        if (names == null) return;

        var subject = GetValidatedTeacherSubject();
        if (string.IsNullOrEmpty(subject)) return;

        var newTeacher = new Teacher(id.Value, names[0], names[1], subject );
        nonNullTeachers.Add(newTeacher);
        Console.WriteLine("Teacher added successfully.");
    }

    private static int? GetValidatedTeacherId(List<Teacher> nonNullTeachers)
    {
        Console.Write("Enter Teacher ID: ");
        var idInput = Console.ReadLine();
        if (!int.TryParse(idInput, out int id))
        {
            Console.WriteLine("Invalid Teacher ID.");
            return null;
        }

        var teacherExists = nonNullTeachers.Any(t => t.GetTeacherId() == id);
        if (teacherExists)
        {
            Console.WriteLine("Teacher with this ID already exists.");
            return null;
        }

        return id;
    }

    private static string[]? GetValidatedTeacherName()
    {
        Console.Write("Enter Teacher Name: ");
        var name = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(name, "Teacher Name cannot be empty.");
        var names = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (names.Length >= 2) return names;
        Console.WriteLine("Please enter both first and last names.");
        return null;

    }

    private static string GetValidatedTeacherSubject()
    {
        Console.Write("Enter Teacher Subject: ");
        var subject = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(subject, "Teacher Subject cannot be empty.");
        return subject;
    }
    private static List<Teacher> ValidateTeacherList(List<Teacher?>? teachers)
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

    public void DisplayTeacherDetails(List<Teacher?>? teachers, object? user)
    {
        ValidateUserPermissions(user);
        var nonNullTeachers = ValidateTeacherList(teachers);

        Console.WriteLine("Choose an option to search by:");
        DisplayMenuOptions(new[] { "Teacher ID", "Teacher Name", "List all teachers", "Teachers by subject" });

        var choice = GetValidatedUserChoice(4);
        Teacher? teacher = null;

        switch (choice)
        {
            case 1:
                teacher = GetTeacherById(nonNullTeachers);
                break;
            case 2:
                teacher = GetTeacherByName(nonNullTeachers);
                break;
            case 3:
                DisplayTeacherNames(nonNullTeachers);
                return;
            case 4:
                DisplayTeachersBySubject(nonNullTeachers);
                return;
            default:
                teacher = null;
                break;
        }

        if (teacher != null)
        {
            Console.WriteLine($"Teacher ID: {teacher.GetTeacherId()}, Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
        }
        else
        {
            PromptToDisplayAllTeachers(nonNullTeachers);
        }
    }

    public void UpdateTeacherDetails(List<Teacher?>? teachers, object? user)
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
                    UpdateTeacherId(nonNullTeachers, user);
                    break;
                case 2:
                    UpdateTeacherSubject(nonNullTeachers, user);
                    break;
                case 3:
                    var teacher = GetTeacherById(nonNullTeachers);
                    if (teacher != null)
                    {
                        UpdateTeacherName(teacher, user);
                    }
                    break;
                case 4:
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please select a valid option.");
                    break;
            }
        }
    }

    public Teacher? GetTeacherById(List<Teacher?>? teachers)
    {
        Console.Write("Enter Teacher ID: ");
        var id = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(id, "Teacher ID cannot be empty.");
        var teacher = teachers?.FirstOrDefault(t => t.GetTeacherId().ToString() == id);
        ValidationHelper.ValidateNotNull(teacher, "Teacher not found.");
        return teacher;
    }

    public Teacher? GetTeacherByName(List<Teacher?>? teachers)
    {
        Console.Write("Enter Teacher Name: ");
        var name = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(name, "Teacher Name cannot be empty.");
        var teacher = teachers?.FirstOrDefault(t => t.GetTeacherFullName().Equals(name, StringComparison.OrdinalIgnoreCase));
        ValidationHelper.ValidateNotNull(teacher, "Teacher not found.");
        return teacher;
    }

    public static void DisplayTeachersBySubject(List<Teacher?>? teachers)
    {
        Console.Write("Enter Subject Name: ");
        var subject = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(subject, "Subject cannot be empty.");
        var filteredTeachers = teachers?.Where(t => t.GetSubject().Equals(subject, StringComparison.OrdinalIgnoreCase)).ToList();
        ValidationHelper.ValidateNotNull(filteredTeachers, "No teachers found for this subject.");
        DisplayTeacherNames(filteredTeachers);
    }

    public void UpdateTeacherId(List<Teacher?>? teachers, object? user)
    {
        var teacher = GetTeacherById(teachers);
        if (teacher == null) return;
        Console.Write("Enter new Teacher ID: ");
        var newId = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(newId, "New Teacher ID cannot be empty.");
        teacher.SetTeacherId(int.Parse(newId));
        Console.WriteLine("Teacher ID updated successfully.");
    }

    public void UpdateTeacherSubject(List<Teacher?>? teachers, object? user)
    {
        var teacher = GetTeacherById(teachers);
        if (teacher == null) return;
        Console.Write("Enter new Subject: ");
        var newSubject = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(newSubject, "New Subject cannot be empty.");
        teacher.SetSubject(newSubject);
        Console.WriteLine("Teacher Subject updated successfully.");
    }

    public void UpdateTeacherName(Teacher teacher, object? user)
    {
        Console.Write("Enter new Teacher Name: ");
        var newName = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(newName, "New Teacher Name cannot be empty.");
        var names = newName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (names.Length >= 2)
        {
            teacher.SetFirstName(names[0]);
            teacher.SetLastName(names[1]);
            Console.WriteLine("Teacher Name updated successfully.");
        }
        else
        {
            Console.WriteLine("Please enter both first and last names.");
        }
    }

    private static void DisplayTeacherNames(List<Teacher?>? teachers)
    {
        ValidationHelper.ValidateNotNull(teachers, "Teacher list cannot be null or empty.");
        Console.WriteLine("Teacher Names:");
        foreach (var teacher in teachers)
        {
            Console.WriteLine($"Teacher ID: {teacher.GetTeacherId()}, Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
        }
    }

    public static void PromptToDisplayAllTeachers(List<Teacher?>? teachers)
    {
        Console.WriteLine("Teacher not found. Would you like to see the list of teachers? (yes/no)");
        if (Console.ReadLine()?.Trim().ToLower() == "yes")
        {
            DisplayTeacherNames(teachers);
        }
    }
}