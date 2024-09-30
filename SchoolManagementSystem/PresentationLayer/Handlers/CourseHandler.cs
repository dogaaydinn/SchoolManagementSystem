using System.Diagnostics;
using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class CourseHandler
{
    private static void DisplayCourseActions(Course course)
    {
        Exceptions.Expectations.CheckCourseNotNull(course);

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

            Console.WriteLine("Would you like to perform another action? (yes/no)");
            var continueResponse = Console.ReadLine()?.Trim().ToLower();
            if (continueResponse != "yes") break;
        }
    }
    public static void DisplayCourseDetails(List<Course>? courses)
    {
        Exceptions.Expectations.CheckCoursesNotNull(courses);

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
                Console.WriteLine("Enter course ID to display details:");
                if (int.TryParse(Console.ReadLine(), out var courseId))
                {
                    if (courses != null) course = courses.Find(c => c.GetCourseId() == courseId);
                }
                else
                    Console.WriteLine("Invalid course ID.");
                break;

            case 2:
                Console.WriteLine("Enter course name to display details:");
                var courseName = Console.ReadLine();
                if (courses != null)
                    course = courses.Find(c =>
                        c.GetCourseName().Equals(courseName, StringComparison.OrdinalIgnoreCase));
                break;

            case 3:
                DisplayCourseNames(courses);
                return;
        }

        if (course != null)
        {
            Console.WriteLine(
                $"Course ID: {course.GetCourseId()}, Name: {course.GetCourseName()}, {course.GetAssignedTeacherName()}, Enrolled Students: {course.GetEnrolledStudents().Count}, Credits: {course.GetCredits()}");
        }
        else
        {
            Console.WriteLine("Course not found. Would you like to see the list of courses? (yes/no)");
            var userResponse = Console.ReadLine()?.Trim().ToLower();

            if (userResponse == "yes") DisplayCourseNames(courses);
        }
    }
    private static void DisplayCourseNames(List<Course>? courses)
    {
        Exceptions.Expectations.CheckCoursesNotNull(courses);

        while (true)
        {
            Console.WriteLine("Course Names:");
            if (courses != null)
                for (var i = 0; i < courses.Count; i++)
                    Console.WriteLine($"{i + 1}. {courses[i].GetCourseName()}");

            Console.WriteLine(
                "Enter the number of the course to view details or perform actions, or type 'exit' to return to the main menu:");
            var input = Console.ReadLine()?.Trim().ToLower();

            if (input == "exit") return;

            Debug.Assert(courses != null, nameof(courses) + " != null");
            if (int.TryParse(input, out var courseIndex) && courseIndex > 0 && courseIndex <= courses.Count)
            {
                var selectedCourse = courses[courseIndex - 1];
                DisplayCourseActions(selectedCourse);
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again.");
                continue;
            }

            break;
        }
    }

    public static void ListStudentsInCourses(List<Course>? courses)
    {
        Exceptions.Expectations.CheckCoursesNotNull(courses);

        while (true)
        {
            Console.WriteLine("Do you want to list students in a specific course? (yes/no)");
            var response = Console.ReadLine()?.Trim().ToLower();

            if (response == "yes")
            {
                Console.WriteLine("Enter course name to list students:");
                var courseName = Console.ReadLine();

                if (courses != null)
                {
                    var course = courses.Find(c =>
                        c.GetCourseName().Equals(courseName, StringComparison.OrdinalIgnoreCase));

                    if (course != null)
                        course.ListStudents();
                    else
                        Console.WriteLine("Course not found.");
                }
            }
            else
            {
                Debug.Assert(courses != null, nameof(courses) + " != null");
                foreach (var course in courses) course.ListStudents();
            }

            Console.WriteLine("Would you like to perform another action? (yes/no)");
            var continueResponse = Console.ReadLine()?.Trim().ToLower();
            if (continueResponse != "yes") break;
        }
    }
    public static void DisplayTotalCourses(List<Course>? courses)
    {
        Exceptions.Expectations.CheckCoursesNotNull(courses);

        if (courses != null)
        {
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
                    DisplayCourseNames(courses);
                    break;

                case 2:
                    DisplayCourseDetails(courses);
                    break;
            }
        }
    }
    public static void DisplayStudents(List<Course>? courses)
    {
        Exceptions.Expectations.CheckCoursesNotNull(courses);

        Console.WriteLine("Do you want to list students in a specific course? (yes/no)");
        var response = Console.ReadLine()?.Trim().ToLower();

        if (response == "yes")
        {
            Console.WriteLine("Enter course name to list students:");
            var courseName = Console.ReadLine();

            var course = courses?.Find(c => c.GetCourseName().Equals(courseName, StringComparison.OrdinalIgnoreCase));

            if (course != null)
                ShowStudents(course.GetEnrolledStudents());
            else
                Console.WriteLine("Course not found.");
        }
        else
        {
            if (courses == null) return;
            foreach (var course in courses)
            {
                Console.WriteLine($"\nCourse: {course.GetCourseName()}");
                ShowStudents(course.GetEnrolledStudents());
            }
        }
    }
    private static void ShowStudents(List<Student?> students)
    {
        Exceptions.Expectations.CheckStudentsNotNull(students);

        if (students.Count == 0)
        {
            Console.WriteLine("No students enrolled.");
            return;
        }

        foreach (var student in students)
            if (student != null)
                Console.WriteLine($"Student ID: {student.GetStudentId()}, Name: {student.GetStudentFullName()}");
    }

    public static void DisplayCourseGrades(List<Course>? courses)
    {
        Exceptions.Expectations.CheckCoursesNotNull(courses);

        Console.WriteLine("Do you want to search by:");
        Console.WriteLine("1. Course ID");
        Console.WriteLine("2. Course Name");
        Console.Write("Enter your choice (1 or 2): ");

        if (!int.TryParse(Console.ReadLine(), out var choice) || (choice != 1 && choice != 2))
        {
            Console.WriteLine("Invalid choice. Please select 1 or 2.");
            return;
        }

        Course? course = null;

        switch (choice)
        {
            case 1:
                Console.WriteLine("Enter course ID to display the grades:");
                if (!int.TryParse(Console.ReadLine(), out var courseId))
                {
                    Console.WriteLine("Invalid course ID. Please try again.");
                    return;
                }

                if (courses != null) course = courses.Find(c => c.GetCourseId() == courseId);
                break;

            case 2:
                Console.WriteLine("Enter course name to display the grades:");
                var courseName = Console.ReadLine();
                Debug.Assert(courses != null, nameof(courses) + " != null");
                course = courses.Find(c => c.GetCourseName().Equals(courseName, StringComparison.OrdinalIgnoreCase));
                break;
        }

        if (course != null)
        {
            course.ListGrades();

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
        else
        {
            Console.WriteLine("Course not found. Would you like to see the list of courses? (yes/no)");
            var userResponse = Console.ReadLine()?.Trim().ToLower();

            if (userResponse == "yes") DisplayCourseNames(courses);
        }
    }
public static void EnrollStudentsInCourses(List<Course>? courses, List<Student> students)
{
    Exceptions.Expectations.CheckCoursesNotNull(courses);

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

        Console.WriteLine("Enter the course ID:");
        if (!int.TryParse(Console.ReadLine(), out var courseId))
        {
            Console.WriteLine("Invalid course ID. Please try again.");
            continue;
        }

        if (courses != null)
        {
            var course = courses.Find(c => c.GetCourseId() == courseId);
            if (course == null)
            {
                Console.WriteLine("Course not found. Please try again.");
                continue;
            }

            Console.WriteLine("Enter the student ID:");
            if (!int.TryParse(Console.ReadLine(), out var studentId))
            {
                Console.WriteLine("Invalid student ID. Please try again.");
                continue;
            }

            var student = students.Find(s => s.GetStudentId() == studentId);
            if (student == null)
            {
                Console.WriteLine("Student not found. Please try again.");
                continue;
            }

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
        }

        Console.WriteLine("Would you like to perform another action? (yes/no)");
        var continueResponse = Console.ReadLine()?.Trim().ToLower();
        if (continueResponse != "yes") break;
    }
}

public static void RemoveStudentInteractive(List<Course>? courses)
{
    Exceptions.Expectations.CheckCoursesNotNull(courses);

    Console.WriteLine("Enter the course ID from which you want to remove a student:");
    if (!int.TryParse(Console.ReadLine(), out int courseId))
    {
        Console.WriteLine("Invalid course ID.");
        return;
    }

    if (courses != null)
    {
        var course = courses.FirstOrDefault(c => c.GetCourseId() == courseId);
        if (course == null)
        {
            Console.WriteLine("Course not found.");
            return;
        }

        Console.WriteLine("Enter the student ID to remove:");
        if (!int.TryParse(Console.ReadLine(), out int studentId))
        {
            Console.WriteLine("Invalid student ID.");
            return;
        }

        var student = course.GetEnrolledStudents().FirstOrDefault(s => s?.GetStudentId() == studentId);
        if (student == null)
        {
            Console.WriteLine("Student not found in this course.");
            return;
        }

        student.UnenrollFromCourse(course);
    }
}
}
    

