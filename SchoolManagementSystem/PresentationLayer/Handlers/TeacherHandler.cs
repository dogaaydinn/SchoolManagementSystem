using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.Interfaces.ActionHandler;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class TeacherHandler
{
    public static void DisplayTeacherDetails(List<Teacher> teachers, object user)
    {
        Exceptions.Expectations.CheckTeachersNotNull(teachers);
        Exceptions.Expectations.CheckHasTeacherPermission(user, isAdmin: true, isTeacherOrAdmin: true, teachers: teachers);

        Console.WriteLine("Do you want to search by:");
        Console.WriteLine("1. Teacher ID");
        Console.WriteLine("2. Teacher Name");
        Console.WriteLine("3. List all teachers");
        Console.WriteLine("4. Teachers by subject");
        Console.Write("Enter your choice (1, 2, 3, or 4): ");

        if (!int.TryParse(Console.ReadLine(), out var choice) || choice < 1 || choice > 4)
        {
            Console.WriteLine("Invalid choice. Please select 1, 2, 3, or 4.");
            return;
        }

        Teacher? teacher = null;

        switch (choice)
        {
            case 1:
                teacher = GetTeacherById(teachers);
                break;
            case 2:
                teacher = GetTeacherByName(teachers);
                break;
            case 3:
                DisplayTeacherNames(teachers);
                return;
            case 4:
                DisplayTeachersBySubject(teachers);
                return;
        }

        if (teacher != null)
        {
            Console.WriteLine($"Teacher ID: {teacher.GetTeacherId()}, Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
        }
        else
        {
            Console.WriteLine("Teacher not found. Would you like to see the list of teachers? (yes/no)");
            var userResponse = Console.ReadLine()?.Trim().ToLower();

            if (userResponse == "yes") DisplayTeacherNames(teachers);
        }
    }
public static void DisplayTeacherNames(List<Teacher> teachers)
{
    Exceptions.Expectations.CheckTeachersNotNull(teachers);

    Console.WriteLine("Teacher Names:");
    foreach (var teacher in teachers)
    {
        Console.WriteLine($"Teacher ID: {teacher.GetTeacherId()}, Teacher Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
    }
}

public static void UpdateTeacherId(List<Teacher> teachers, object user)
{
    Exceptions.Expectations.CheckTeachersNotNull(teachers);
    Exceptions.Expectations.CheckHasPermission(user, isAdmin: true);

    var teacher = GetTeacherById(teachers);
    if (teacher == null) return;

    Console.Write("Enter the new Teacher ID: ");
    if (int.TryParse(Console.ReadLine(), out var newTeacherId) && newTeacherId != teacher.GetTeacherId())
    {
        teacher.SetTeacherId(newTeacherId);
        Console.WriteLine("Teacher ID updated successfully.");
    }
    else
    {
        Console.WriteLine("Invalid input for Teacher ID or the new ID is the same as the current one.");
    }
}
private static Teacher? GetTeacherById(List<Teacher> teachers)
{
    Console.Write("Enter teacher ID to display details: ");
    var input = Console.ReadLine();

    if (int.TryParse(input, out var teacherId))
    {
        var teacher = teachers.Find(t => t.GetTeacherId() == teacherId);
        if (teacher != null)
        {
            return teacher;
        }
        Console.WriteLine("Teacher not found.");
        return null;
    }
    Console.WriteLine("Invalid teacher ID.");
    return null;
}
private static Teacher? GetTeacherByName(List<Teacher> teachers)
{
    Console.WriteLine("Enter teacher name to display details:");
    var teacherName = Console.ReadLine();
    return teachers.Find(t => t.GetTeacherFullName().Equals(teacherName, StringComparison.OrdinalIgnoreCase));
}

private static void DisplayTeachersBySubject(List<Teacher> teachers)
{
    Console.WriteLine("Enter subject to list teachers:");
    var subject = Console.ReadLine();
    var subjectTeachers = teachers.FindAll(t => t.GetSubject().Equals(subject, StringComparison.OrdinalIgnoreCase));
    DisplayTeacherNames(subjectTeachers);
}
   private static Teacher? GetTeacherByIdOrName(List<Teacher> teachers)
{
    Console.Write("Enter the Teacher Name: ");
    var teacherName = Console.ReadLine();
    return teachers.Find(t => t.GetTeacherFullName().Equals(teacherName, StringComparison.OrdinalIgnoreCase));
}

private static void UpdateTeacherSubject(List<Teacher> teachers, object user)
{
    Exceptions.Expectations.CheckTeachersNotNull(teachers);
    Exceptions.Expectations.CheckHasTeacherPermission(user, isAdmin: false, isTeacherOrAdmin: true, teachers: teachers);

    var teacher = GetTeacherById(teachers);
    if (teacher == null) return;

    Console.Write("Enter the new Subject: ");
    var newSubject = Console.ReadLine();
    if (!string.IsNullOrEmpty(newSubject))
    {
        teacher.SetSubject(newSubject);
        Console.WriteLine("Subject updated successfully.");
    }
    else
    {
        Console.WriteLine("Invalid input for Subject.");
    }
}

public static void DemonstrateTeacherMethods(Teacher? teacher, object user)
{
    Exceptions.Expectations.CheckTeacherNotNull(teacher);
    Exceptions.Expectations.CheckHasTeacherPermission(user, isAdmin: false, isTeacherOrAdmin: true, teachers: null);

    if (teacher != null)
    {
        ActionDemonstrator.DemonstrateActions(teacher);
    }
    else
    {
        Console.WriteLine("Teacher cannot be null.");
    }
}

public static void UpdateTeacherDetails(List<Teacher> teachers, object user)
{
    Exceptions.Expectations.CheckTeachersNotNull(teachers);
    Exceptions.Expectations.CheckHasTeacherPermission(user, isAdmin: true, isTeacherOrAdmin: false, teachers: teachers);

    while (true)
    {
        Console.WriteLine("\nTeacher Update Menu:");
        Console.WriteLine("1. Update Teacher ID");
        Console.WriteLine("2. Update Teacher Subject");
        Console.WriteLine("3. Exit");
        Console.Write("Enter your choice: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                UpdateTeacherId(teachers, user);
                break;
            case "2":
                UpdateTeacherSubject(teachers, user);
                break;
            case "3":
                return;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }
}

}