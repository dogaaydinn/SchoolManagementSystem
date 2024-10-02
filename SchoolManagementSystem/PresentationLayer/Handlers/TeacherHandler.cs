using System.Diagnostics;
using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers.ActionHandler;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class TeacherHandler
{
    public static void DisplayTeacherDetails(List<Teacher?>? teachers, object? user)
    {
        var nonNullTeachers = teachers?.OfType<Teacher>().ToList();
        Exceptions.CheckTeachersNotNull(nonNullTeachers);
        Exceptions.CheckHasTeacherPermission(user, isAdmin: true, isTeacherOrAdmin: true, teachers: nonNullTeachers);
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
    public static void DisplayAllTeachers(List<Teacher?>? teachers)
    {
        if (teachers == null || teachers.Count == 0)
        {
            Console.WriteLine("No teachers available.");
            return;
        }

        foreach (var teacher in teachers)
        {
            Console.WriteLine($"Teacher ID: {teacher?.GetTeacherId()}, Name: {teacher?.GetTeacherFullName()}, Subject: {teacher?.GetSubject()}");
        }
    }
    public static void DisplayTeacherNames(List<Teacher?>? teachers)
    {
        var nonNullTeachers = teachers?.OfType<Teacher>().ToList();
        Exceptions.CheckTeachersNotNull(nonNullTeachers);

        Console.WriteLine("Teacher Names:");
        Debug.Assert(nonNullTeachers != null, nameof(nonNullTeachers) + " != null");
        foreach (var teacher in nonNullTeachers.Where(teacher => true))
        {
            Console.WriteLine($"Teacher ID: {teacher.GetTeacherId()}, Teacher Name: {teacher.GetTeacherFullName()}, Subject: {teacher.GetSubject()}");
        }
    }

    public static Teacher? GetTeacherById(List<Teacher?>? teachers)
    {
        Console.Write("Enter teacher ID to display details: ");
        var input = Console.ReadLine();
        if (int.TryParse(input, out var teacherId))
        {
            var teacher = teachers?.Find(t => t?.GetTeacherId() == teacherId);
            if (teacher != null)
            {
                return teacher;
            }
        }
        Console.WriteLine("Invalid teacher ID.");
        return null;
    }

    public static void UpdateTeacherId(List<Teacher?>? teachers, object? user)
    {
        var nonNullTeachers = teachers?.OfType<Teacher>().ToList();
        Exceptions.CheckTeachersNotNull(nonNullTeachers);
        Exceptions.CheckHasPermission(user, isAdmin: true);

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

    public static Teacher? GetTeacherByName(List<Teacher?>? teachers)
    {
        Console.WriteLine("Enter teacher name to display details:");
        var teacherName = Console.ReadLine();
        return teachers.Find(t => t.GetTeacherFullName().Equals(teacherName, StringComparison.OrdinalIgnoreCase));
    }

    public static void DisplayTeachersBySubject(List<Teacher?>? teachers)
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

    public static void UpdateTeacherSubject(List<Teacher?>? teachers, object? user)
    {
        Exceptions.CheckTeachersNotNull(teachers);
        Exceptions.CheckHasTeacherPermission(user, isAdmin: false, isTeacherOrAdmin: true, teachers: teachers);

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

    public static void DemonstrateTeacherMethods(Teacher? teacher, object? user)
    {
        Exceptions.CheckTeacherNotNull(teacher);
        Exceptions.CheckHasTeacherPermission(user, isAdmin: false, isTeacherOrAdmin: true, teachers: null);

        if (teacher != null)
        {
            PersonActionDemonstrator.DemonstrateActions(teacher);
        }
        else
        {
            Console.WriteLine("Teacher cannot be null.");
        }
    }

    public static void UpdateTeacherDetails(List<Teacher?>? teachers, object? user)
    {
        Exceptions.CheckTeachersNotNull(teachers);
        Exceptions.CheckHasTeacherPermission(user, isAdmin: true, isTeacherOrAdmin: false, teachers: teachers);

        while (true)
        {
            Console.WriteLine("\nTeacher Update Menu:");
            Console.WriteLine("1. Update Teacher ID");
            Console.WriteLine("2. Update Teacher Subject");
            Console.WriteLine("2. Update Teacher Name ");
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
    public static void UpdateTeacherName(Teacher teacher, IUser user)
    {
        Exceptions.CheckTeacherNotNull(teacher);
        Exceptions.CheckHasPermission(user, isAdmin: true);

        Console.Write("Enter the new Teacher Name: ");
        var newTeacherName = Console.ReadLine();
        if (!string.IsNullOrEmpty(newTeacherName))
        {
            teacher.SetTeacherFullName(newTeacherName);
            Console.WriteLine("Teacher name updated successfully.");
        }
        else
        {
            Console.WriteLine("Invalid input for Teacher Name.");
        }
    }

    public static void AddNewTeacher(List<Teacher?>? teachers, IUser user)
    {
        Exceptions.CheckTeachersNotNull(teachers);
        Exceptions.CheckHasPermission(user, isAdmin: true);

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

        Console.Write("Enter teacher ID: ");
        if (!int.TryParse(Console.ReadLine(), out var teacherId))
        {
            Console.WriteLine("Invalid teacher ID.");
            return;
        }

        Console.Write("Enter subject: ");
        var subject = Console.ReadLine();

        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(subject))
        {
            Console.WriteLine("Invalid input. All fields are required.");
            return;
        }

        var newTeacher = new Teacher(firstName, lastName, dateOfBirth, teacherId, subject);
        teachers?.Add(newTeacher);
        Console.WriteLine("New teacher added successfully.");
    }

    public static void RemoveTeacher(List<Teacher?>? teachers, Teacher? teacher, IUser user)
    {
        Exceptions.CheckTeachersNotNull(teachers);
        Exceptions.CheckTeacherNotNull(teacher);
        Exceptions.CheckHasPermission(user, isAdmin: true);

        Debug.Assert(teachers != null, nameof(teachers) + " != null");
        Console.WriteLine(teachers.Remove(teacher) ? "Teacher removed successfully." : "Failed to remove teacher.");
    }
}