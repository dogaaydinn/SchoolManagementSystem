using SchoolManagementSystem.BusinessLogicLayer.Validations;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class TeacherHandler 
{
    public static void DisplayTeacherDetails(List<Teacher?> teachers, object user)
    {
        ValidateUser(user);
        var nonNullTeachers = GetNonNullTeachers(teachers);

        Console.WriteLine("Choose an option to search by:");
        DisplayMenuOptions(new[] { "Teacher ID", "Teacher Name", "List all teachers", "Teachers by subject" });

        var choice = GetValidatedUserChoice(4);
        Teacher teacher = null;

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

    public static void UpdateTeacherDetails(List<Teacher?> teachers, object user)
    {
        ValidateUser(user);
        var nonNullTeachers = GetNonNullTeachers(teachers);

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
    public static void DisplayAllTeachers(List<Teacher?> teachers)
    {
        if (teachers == null || !teachers.Any())
        {
            Console.WriteLine("No teachers available.");
            return;
        }

        foreach (var teacher in teachers.OfType<Teacher>())
        {
            Console.WriteLine($"ID: {teacher.GetTeacherId()}, Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
        }
    }
    public static Teacher GetTeacherById(List<Teacher?> teachers)
    {
        Console.Write("Enter Teacher ID: ");
        var id = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(id, "Teacher ID cannot be empty.");
        var teacher = teachers?.FirstOrDefault(t => t.GetTeacherId().ToString() == id);
        ValidationHelper.ValidateNotNull(teacher, "Teacher not found.");
        return teacher;
    }

    public static Teacher GetTeacherByName(List<Teacher?> teachers)
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
        ValidationHelper.ValidateList(filteredTeachers, "No teachers found for this subject.");
        DisplayTeacherNames(filteredTeachers);
    }

    public static void UpdateTeacherId(List<Teacher?> teachers, object user)
    {
        var teacher = GetTeacherById(teachers);
        if (teacher == null) return;
        Console.Write("Enter new Teacher ID: ");
        var newId = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(newId, "New Teacher ID cannot be empty.");
        teacher.SetTeacherId(int.Parse(newId));
        Console.WriteLine("Teacher ID updated successfully.");
    }
    public static void GetTeacherByCourse(List<Teacher?>? teachers, List<Student?>? students)
    {
        Console.Write("Enter Course ID: ");
        var courseIdInput = Console.ReadLine();
        if (int.TryParse(courseIdInput, out int courseId))
        {
            var course = DataProvider.GetCourses(teachers, students)
                .FirstOrDefault(c => c.GetCourseId() == courseId);
            if (course != null)
            {
                var teacher = teachers?.FirstOrDefault(t => t.GetTeacherId() == Course.GetTeacherId());
                if (teacher != null)
                {
                    DisplayTeacherDetails(new List<Teacher?> { teacher }, null);
                }
                else
                {
                    Console.WriteLine("Teacher not found for the given course.");
                }
            }
            else
            {
                Console.WriteLine("Course not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid Course ID.");
        }
        
    }

    public static void RemoveTeacher(List<Teacher?> teachers, Teacher teacher, IUser user)
    {
        ValidateUser(user);
        var nonNullTeachers = GetNonNullTeachers(teachers);

        Console.Write("Enter Teacher ID to remove: ");
        var idInput = Console.ReadLine();
        if (!int.TryParse(idInput, out int id))
        {
            Console.WriteLine("Invalid Teacher ID.");
            return;
        }

        var teacherToRemove = nonNullTeachers.FirstOrDefault(t => t.GetTeacherId() == id);
        if (teacherToRemove == null)
        {
            Console.WriteLine("Teacher not found.");
            return;
        }

        nonNullTeachers.Remove(teacherToRemove);
        Console.WriteLine("Teacher removed successfully.");
    }
    public static void UpdateTeacherSubject(List<Teacher?> teachers, object user)
    {
        var teacher = GetTeacherById(teachers);
        if (teacher == null) return;
        Console.Write("Enter new Subject: ");
        var newSubject = Console.ReadLine();
        ValidationHelper.ValidateNotEmpty(newSubject, "New Subject cannot be empty.");
        teacher.SetSubject(newSubject);
        Console.WriteLine("Teacher Subject updated successfully.");
    }
    
    public static void AddNewTeacher(List<Teacher?> teachers, object user)
    {
        ValidateUser(user);
        var nonNullTeachers = GetNonNullTeachers(teachers);
        
        var id = GetValidatedTeacherId(nonNullTeachers);
        if (id == null) return;

        var names = GetValidatedTeacherName();
        if (names == null) return;

        Console.Write("Enter Teacher Subject ID: ");
        var subjectInput = Console.ReadLine();
        if (!int.TryParse(subjectInput, out int subjectId))
        {
            Console.WriteLine("Invalid Subject ID.");
            return;
        }

        var hireDate = InputHelper.GetValidatedDateInput("Enter Hire Date (yyyy-MM-dd):");
        var newTeacher = new Teacher(names[0], names[1], hireDate, subjectId, id.Value.ToString());
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
        if (!teacherExists) return id;
        Console.WriteLine("Teacher with this ID already exists.");
        return null;

    }
    public static void RemoveTeacher(List<Teacher?> teachers, object user)
    {
        ValidateUser(user);
        var nonNullTeachers = GetNonNullTeachers(teachers);

        Console.Write("Enter Teacher ID to remove: ");
        var idInput = Console.ReadLine();
        if (!int.TryParse(idInput, out int id))
        {
            Console.WriteLine("Invalid Teacher ID.");
            return;
        }

        var teacherToRemove = nonNullTeachers.FirstOrDefault(t => t.GetTeacherId() == id);
        if (teacherToRemove == null)
        {
            Console.WriteLine("Teacher not found.");
            return;
        }

        nonNullTeachers.Remove(teacherToRemove);
        Console.WriteLine("Teacher removed successfully.");
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

    public static void UpdateTeacherName(Teacher teacher, object user)
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

    public static void DisplayTeacherNames(List<Teacher?> teachers)
    {
        ValidationHelper.ValidateList(teachers, "Teacher list cannot be null or empty.");
        Console.WriteLine("Teacher Names:");
        foreach (var teacher in teachers)
        {
            Console.WriteLine($"Teacher ID: {teacher.GetTeacherId()}, Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
        }
    }

    private static void PromptToDisplayAllTeachers(List<Teacher?> teachers)
    {
        Console.WriteLine("Teacher not found. Would you like to see the list of teachers? (yes/no)");
        if (Console.ReadLine()?.Trim().ToLower() == "yes")
        {
            DisplayTeacherNames(teachers);
        }
    }

    public static void DisplayTeacherCourses(List<Teacher?> teachers, object user)
    {
        if (teachers == null || !teachers.Any())
        {
            Console.WriteLine("No teachers available.");
            return;
        }

        Console.Write("Enter Teacher ID: ");
        var idInput = Console.ReadLine();
        if (!int.TryParse(idInput, out int id))
        {
            Console.WriteLine("Invalid Teacher ID.");
            return;
        }

        var teacher = teachers.FirstOrDefault(t => t?.GetTeacherId() == id);
        if (teacher == null)
        {
            Console.WriteLine("Teacher not found.");
            return;
        }

        var students = DataProvider.GetStudents(null); 
        var courses = DataProvider.GetCourses(new List<Teacher?> { teacher }, students)
            .Where(c => Course.GetTeacherId() == teacher.GetTeacherId())
            .ToList();

        if (!courses.Any())
        {
            Console.WriteLine("This teacher has no courses assigned.");
            return;
        }

        Console.WriteLine($"Courses for {teacher.GetTeacherFullName()}:");
        foreach (var course in courses)
        {
            Console.WriteLine($"- {course.GetCourseName()} (ID: {course.GetCourseId()})");
        }
    }

    private static void DisplayMenuOptions(string[] options)
    {
        ValidationHelper.ValidateNotNull(options, "Options cannot be null.");
        for (var i = 0; i < options.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {options[i]}");
        }
        Console.Write("Enter your choice: ");
    }

    private static int GetValidatedUserChoice(int maxOptions)
    {
        if (maxOptions <= 0) throw new ArgumentOutOfRangeException(nameof(maxOptions), "Max options must be greater than zero.");

        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > maxOptions)
        {
            Console.WriteLine($"Invalid choice. Please select a number between 1 and {maxOptions}.");
        }
        return choice;
    }

    private static void ValidateUser(object user)
    {
        ValidationHelper.ValidateNotNull(user, "User cannot be null.");
        ValidationHelper.ValidateUserPermissions(user, isAdmin: true);
    }

    private static List<Teacher> GetNonNullTeachers(List<Teacher?> teachers)
    {
        var nonNullTeachers = teachers?.OfType<Teacher>().ToList();
        ValidationHelper.ValidateList(nonNullTeachers, "Teacher list cannot be null or empty.");
        return nonNullTeachers!;
    }
}