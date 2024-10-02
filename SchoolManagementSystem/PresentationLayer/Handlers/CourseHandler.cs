using System.Diagnostics;
using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class CourseHandler
{
public static void DisplayCourseActions(Course course, object? user)
{
    Exceptions.CheckCourseNotNull(course);

    if (!HasPermission(user, course))
    {
        throw new Exceptions.PermissionDeniedException("You do not have permission to perform actions on courses.");
    }

    while (true)
    {
        var courseName = course.GetCourseName();
        if (string.IsNullOrEmpty(courseName))
        {
            Console.WriteLine("Course name is invalid.");
            return;
        }

        Console.WriteLine($"\nActions for {courseName}:");
        Console.WriteLine("1. Enroll Students");
        Console.WriteLine("2. List Students");
        Console.WriteLine("3. View Grades");
        Console.WriteLine("4. Back to Course Names");
        Console.Write("Enter your choice (1, 2, 3, or 4): ");

        var input = Console.ReadLine();
        if (!int.TryParse(input, out var choice) || choice < 1 || choice > 4)
        {
            Console.WriteLine("Invalid choice. Please select 1, 2, 3, or 4.");
            continue;
        }

        switch (choice)
        {
            case 1:
                // Enroll student logic here if needed
                Console.WriteLine("Enroll student logic not implemented.");
                break;

            case 2:
                course.ListStudents();
                break;

            case 3:
                course.ListGrades();
                break;

            case 4:
                return;
        }

        if (!AskToContinue()) break;
    }
}
public static void DisplayCourseDetails(List<Course>? courses, object? user)
{
    Exceptions.CheckCoursesNotNull(courses);

    if (!HasPermission(user, isTeacherOrAdmin: false))
    {
        throw new Exceptions.PermissionDeniedException("You do not have permission to view course details.");
    }

    Console.WriteLine("Do you want to search by:");
    Console.WriteLine("1. Course ID");
    Console.WriteLine("2. Course Name");
    Console.WriteLine("3. List all courses");
    Console.Write("Enter your choice (1, 2, or 3): ");

    if (!int.TryParse(Console.ReadLine(), out var choice) || choice < 1 || choice > 3)
    {
        Console.WriteLine("Invalid choice. Please select 1, 2, or 3.");
        return;
    }

    Course? course = null;

    switch (choice)
    {
        case 1:
            course = GetCourseById(courses);
            break;

        case 2:
            course = GetCourseByName(courses);
            break;

        case 3:
            DisplayCourseNames(courses, user);
            return;
    }

    if (course != null)
    {
        DisplayCourseInfo(course);
    }
    else
    {
        Console.WriteLine("Course not found. Would you like to see the list of courses? (yes/no)");
        if (Console.ReadLine()?.Trim().ToLower() == "yes")
        {
            DisplayCourseNames(courses, user);
        }
    }
}
public static void DisplayCourseNames(List<Course>? courses, object? user)
{
    Exceptions.CheckCoursesNotNull(courses);

    while (true)
    {
        Console.WriteLine("Course Names:");
        if (courses != null)
        {
            for (var i = 0; i < courses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {courses[i].GetCourseName()}");
            }
        }

        Console.WriteLine("Enter the number of the course to view details or perform actions, or type 'exit' to return to the main menu:");
        var input = Console.ReadLine()?.Trim().ToLower();

        if (input == "exit") return;

        if (courses != null && int.TryParse(input, out var courseIndex) && courseIndex > 0 && courseIndex <= courses.Count)
        {
            var selectedCourse = courses[courseIndex - 1];
            DisplayCourseActions(selectedCourse, user);
        }
        else
        {
            Console.WriteLine("Invalid input. Please try again.");
        }
    }
}
private static bool HasPermission(object? user, Course? course = null)
{
    return user switch
    {
        Admin => true,
        Teacher teacher when (course == null || teacher.GetTeacherId() == course.GetAssignedTeacher()) => true,
        Student student when course?.GetEnrolledStudents() != null &&
                             (course.GetEnrolledStudents() ?? throw new InvalidOperationException()).Any(s => s?.GetStudentId() == student.GetStudentId()) => true,
        _ => false
    };
}
private static void DisplayCourseInfo(Course course)
{
    var courseId = course.GetCourseId();
    var courseName = course.GetCourseName();
    var assignedTeacherName = course.GetAssignedTeacherName();
    var enrolledStudents = course.GetEnrolledStudents();
    var credits = course.GetCredits();

    if (enrolledStudents == null)
    {
        Console.WriteLine("Course information is incomplete.");
        return;
    }

    Console.WriteLine($"Course ID: {courseId}, Name: {courseName}, Teacher: {assignedTeacherName}, Enrolled Students: {enrolledStudents.Count}, Credits: {credits}");
}
private static bool AskToContinue()
{
    Console.WriteLine("Would you like to perform another action? (yes/no)");
    return Console.ReadLine()?.Trim().ToLower() == "yes";
}
public static void ListStudentsInCourses(List<Course>? courses, object? user)
{
    Exceptions.CheckCoursesNotNull(courses);

    if (!HasPermission(user, isTeacherOrAdmin: false))
    {
        throw new Exceptions.PermissionDeniedException("You do not have permission to list students in courses.");
    }

    while (true)
    {
        Console.WriteLine("Do you want to list students in a specific course? (yes/no)");
        var response = Console.ReadLine()?.Trim().ToLower();

        if (response == "yes")
        {
            var course = GetCourseByName(courses);
            if (course != null)
            {
                if (HasPermission(user, course))
                {
                    course.ListStudents();
                }
                else
                {
                    Console.WriteLine("You do not have permission to list students in this course.");
                }
            }
            else
            {
                Console.WriteLine("Course not found.");
            }
        }
        else
        {
            ListAllStudentsInCourses(courses, user);
        }

        if (!AskToContinue()) break;
    }
}
public static void DisplayTotalCourses(List<Course>? courses, object? user)
{
    Exceptions.CheckCoursesNotNull(courses);

    if (!HasPermission(user, isTeacherOrAdmin: false))
    {
        throw new Exceptions.PermissionDeniedException("You do not have permission to list students in courses.");
    }

    if (courses == null) return;
    Console.WriteLine($"Total courses: {courses.Count}");

    Console.WriteLine("Would you like to:");
    Console.WriteLine("1. List all courses");
    Console.WriteLine("2. View course details");
    Console.Write("Enter your choice (1 or 2): ");

    if (!int.TryParse(Console.ReadLine(), out var choice) || (choice != 1 && choice != 2))
    {
        Console.WriteLine("Invalid choice. Please select 1 or 2.");
        return;
    }

    switch (choice)
    {
        case 1:
            DisplayCourseNames(courses, user);
            break;
        case 2:
            DisplayCourseDetails(courses, user);
            break;
    }
}
public static void DisplayStudents(List<Course>? courses, object? user)
{
    Exceptions.CheckCoursesNotNull(courses);

    if (!HasPermission(user, isTeacherOrAdmin: false))
    {
        throw new Exceptions.PermissionDeniedException("You do not have permission to list students in courses.");
    }

    Console.WriteLine("Do you want to list students in a specific course? (yes/no)");
    var response = Console.ReadLine()?.Trim().ToLower();

    if (response == "yes")
    {
        var course = GetCourseByName(courses);
        if (course != null)
        {
            ShowStudents(course.GetEnrolledStudents());
        }
        else
        {
            Console.WriteLine("Course not found.");
        }
    }
    else
    {
        ListAllStudentsInCourses(courses, user);
    }
}

private static void ShowStudents(List<Student?>? students)
{
    Exceptions.CheckStudentsNotNull(students);

    if (students == null || students.Count == 0)
    {
        Console.WriteLine("No students enrolled.");
        return;
    }

    var sortedStudents = students
        .Where(student => student != null)
        .OrderBy(student => student?.GetStudentFullName())
        .ToList();

    Console.WriteLine("Enrolled Students:");
    Console.WriteLine("------------------");

    foreach (var student in sortedStudents.OfType<Student>().Where(student => !string.IsNullOrEmpty(student.GetStudentFullName())))
    {
        Console.WriteLine($"Student ID: {student.GetStudentId()}, Name: {student.GetStudentFullName()}");
    }
}
public static void ListAllStudentsInCourses(List<Course>? courses, object? user)
{
    Debug.Assert(courses != null, nameof(courses) + " != null");
    foreach (var course in courses)
    {
        if (HasPermission(user, course))
        {
            course.ListStudents();
        }
        else
        {
            Console.WriteLine($"You do not have permission to list students in the course: {course.GetCourseName()}.");
        }
    }
}
public static void DisplayCourseGrades(List<Course>? courses, object? user)
{
    Exceptions.CheckCoursesNotNull(courses);

    if (!HasPermission(user, isTeacherOrAdmin: false))
    {
        throw new Exceptions.PermissionDeniedException("You do not have permission to view course grades.");
    }

    var course = GetCourseFromUserInput(courses);
    if (course != null)
    {
        course.ListGrades();
        DisplayGrades(course);
    }
    else
    {
        Console.WriteLine("Course not found. Would you like to see the list of courses? (yes/no)");
        if (Console.ReadLine()?.Trim().ToLower() == "yes")
            DisplayCourseNames(courses, user);
    }
}
public static void EnrollStudentsInCourses(List<Course>? courses, List<Student?> students, object? user)
{
    Exceptions.CheckCoursesNotNull(courses);

if (!HasPermission(user, isTeacherOrAdmin: false))
    {
        throw new Exceptions.PermissionDeniedException("You do not have permission to enroll students in courses.");
    }

    while (true)
    {
        Console.WriteLine("Would you like to:");
        Console.WriteLine("1. Enroll a student in a course");
        Console.WriteLine("2. Unenroll a student from a course");
        Console.WriteLine("3. Exit");
        Console.Write("Enter your choice (1, 2, or 3): ");

        if (!int.TryParse(Console.ReadLine(), out var choice) || choice < 1 || choice > 3)
        {
            Console.WriteLine("Invalid choice. Please select 1, 2, or 3.");
            continue;
        }

        if (choice == 3) break;

        var course = GetCourseById(courses);
        if (course == null) continue;

        var nonNullStudents = students.OfType<Student>().ToList();
        var student = GetStudentById(nonNullStudents);
        if (student == null) continue;

        switch (choice)
        {
            case 1:
                course.EnrollStudent(student);
                Console.WriteLine($"{student.GetStudentFullName()} has been enrolled in {course.GetCourseName()}.");
                break;
            case 2:
                course.UnenrollStudent(student);
                Console.WriteLine($"{student.GetStudentFullName()} has been unenrolled from {course.GetCourseName()}.");
                break;
        }

        if (!AskToContinue()) break;
    }
}
public static void RemoveStudentInteractive(List<Course>? courses, object? user)
{
    Exceptions.CheckCoursesNotNull(courses);

    if (!HasPermission(user, isTeacherOrAdmin: false))
    {
        throw new Exceptions.PermissionDeniedException("You do not have permission to remove students from courses.");
    }
    

    var course = GetCourseById(courses);
    if (course == null) return;

    var student = GetStudentFromCourse(course);
    if (student == null) return;

    student.UnenrollFromCourse(course);
    Console.WriteLine($"{student.GetStudentFullName()} has been removed from {course.GetCourseName()}.");
}
private static bool HasPermission(object? user, bool isTeacherOrAdmin = false)
{
    if (user is Admin) return true;
    switch (isTeacherOrAdmin)
    {
        case true when user is Teacher:
        case false when user is Student:
            return true;
        default:
            return false;
    }
}
public static Course? GetCourseFromUserInput(List<Course>? courses)
{
    Console.WriteLine("Do you want to search by:");
    Console.WriteLine("1. Course ID");
    Console.WriteLine("2. Course Name");
    Console.Write("Enter your choice (1 or 2): ");

    if (int.TryParse(Console.ReadLine(), out var choice) && choice is 1 or 2)
    {
        return choice switch
        {
            1 => GetCourseById(courses),
            2 => GetCourseByName(courses),
            _ => throw new ArgumentOutOfRangeException(nameof(choice), "The choice must be either 1 or 2.")
        };
    }
    Console.WriteLine("Invalid choice. Please select 1 or 2.");
    return null;

}
public static Course? GetCourseById(List<Course>? courses)
{
    Console.WriteLine("Enter course ID:");
    if (int.TryParse(Console.ReadLine(), out var courseId)) return courses?.Find(c => c.GetCourseId() == courseId);
    Console.WriteLine("Invalid course ID. Please try again.");
    return null;

}
private static Course? GetCourseByName(List<Course>? courses)
{
    Console.WriteLine("Enter course name:");
    var courseName = Console.ReadLine();
    return courses?.Find(c => c.GetCourseName().Equals(courseName, StringComparison.OrdinalIgnoreCase));
}
private static Student? GetStudentById(List<Student> students)
{
    Console.WriteLine("Enter student ID:");
    if (int.TryParse(Console.ReadLine(), out var studentId)) return students.Find(s => s.GetStudentId() == studentId);
    Console.WriteLine("Invalid student ID. Please try again.");
    return null;
}
public static Student? GetStudentFromCourse(Course course)
{
    Console.WriteLine("Enter the student ID to remove:");
    if (int.TryParse(Console.ReadLine(), out var studentId))
        return (course.GetEnrolledStudents() ?? throw new InvalidOperationException()).FirstOrDefault(s => s?.GetStudentId() == studentId);
    Console.WriteLine("Invalid student ID.");
    return null;
}
public static void DisplayGrades(Course course)
{
    var grades = course.GetGrades();
    if (grades.Count > 0)
    {
        var averageGrade = grades.Average();
        Console.WriteLine($"Average Grade: {averageGrade:F2}");
    }
    else
    {
        Console.WriteLine("No grades available to calculate the average.");
    }

    Console.WriteLine($"Course Credits: {course.GetCredits()}");
}
public static void UpdateCourseId(Course course, IUser? user)
{
    Exceptions.CheckUserPermission(user);

    Console.Write("Enter new Course ID: ");
    var newCourseIdInput = Console.ReadLine();

    if (string.IsNullOrEmpty(newCourseIdInput))
    {
        Console.WriteLine("Course ID cannot be empty.");
        return;
    }

    if (!int.TryParse(newCourseIdInput, out var newCourseId))
    {
        Console.WriteLine("Invalid Course ID. Please enter a valid integer.");
        return;
    }

    course.SetCourseId(newCourseId);
    Console.WriteLine("Course ID updated successfully.");
}
public static void UpdateCourseName(Course course, IUser? user)
{
    Exceptions.CheckUserPermission(user);

    Console.Write("Enter new Course Name: ");
    var newCourseName = Console.ReadLine();

    if (string.IsNullOrEmpty(newCourseName))
    {
        Console.WriteLine("Course name cannot be empty.");
        return;
    }

    course.SetCourseName(newCourseName);
    Console.WriteLine("Course name updated successfully.");
}
}