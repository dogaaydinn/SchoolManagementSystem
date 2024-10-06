using SchoolManagementSystem.BusinessLogicLayer.Exceptions;
using SchoolManagementSystem.BusinessLogicLayer.Validations;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Helpers;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class CourseHandler
{
    public static void DisplayCourseDetails(IEnumerable<Course?>? courses, object user)
    {
        ValidationHelper.ValidateNotNull(courses, "Courses");
        ValidationHelper.ValidateUserPermission(user);

        Console.WriteLine("Do you want to search by:");
        Console.WriteLine("1. Course ID");
        Console.WriteLine("2. Course Name");
        Console.WriteLine("3. List all courses");

        var choice = InputHelper.GetValidatedIntInput("Enter your choice (1-3):");
        Course? course = null;
        var courseHelper = new CourseHelper();

        switch (choice)
        {
            case 1:
                course = CourseHelper.GetCourseById(courses.ToList());
                break;
            case 2:
                course = CourseHelper.GetCourseByName(courses.ToList());
                break;
            case 3:
                DisplayCourseNames(courses, user);
                return;
            default:
                Console.WriteLine("Invalid choice. Please select 1, 2, or 3.");
                return;
        }

        if (course != null)
            DisplayCourseInfo(course);
        else
            PromptAndDisplayAllCourses(courses, user);
    }

    private static void PromptAndDisplayAllCourses(IEnumerable<Course?>? courses, object user)
    {
        ValidationHelper.ValidateNotNull(courses, "Courses");
        ValidationHelper.ValidateUserPermission(user);

        Console.WriteLine("Course not found. Would you like to see the list of courses? (yes/no)");
        if (InputHelper.GetValidatedYesNoInput() == "yes") DisplayCourseNames(courses, user);
    }

    private static void DisplayCourseInfo(Course? course)
    {
        ValidationHelper.ValidateNotNull(course, "Course");
        Console.WriteLine($"Course ID: {course.GetCourseId()}, Name: {course.GetCourseName()}, " +
                          $"Teacher: {Course.GetAssignedTeacherName()}, " +
                          $"Enrolled Students: {course.GetEnrolledStudents()?.Count ?? 0}, " +
                          $"Credits: {course.GetCredits()}");
    }

    public static void DisplayCourseNames(IEnumerable<Course?>? courses, object user)
    {
        ValidationHelper.ValidateNotNull(courses, "Courses");
        ValidationHelper.ValidateUserPermission(user);

        Console.WriteLine("Listing all courses:");
        ListEntities(courses.OfType<Course>(), "Course",
            course => { Console.WriteLine($"Course ID: {course.GetCourseId()}, Name: {course.GetCourseName()}"); });
    }

    public static void DisplayCourseGrades(IEnumerable<Course> courses, IEnumerable<Student?> students)
    {
        ValidationHelper.ValidateNotNull(courses, "Courses list cannot be null.");
        ValidationHelper.ValidateNotNull(students, "Students list cannot be null.");

        if (!courses.Any())
        {
            Console.WriteLine("No courses available.");
            return;
        }

        foreach (var course in courses.OfType<Course>())
        {
            Console.WriteLine($"Course: {course.GetCourseName()} (ID: {course.GetCourseId()})");
            var enrolledStudents = course.GetEnrolledStudents();
            if (enrolledStudents == null || !enrolledStudents.Any())
            {
                Console.WriteLine("  No students enrolled in this course.");
                continue;
            }

            ListEntities(enrolledStudents.OfType<Student>(), "Student",
                student =>
                {
                    Console.WriteLine(
                        $"  Student ID: {student.GetStudentId()}, Name: {student.GetStudentFullName()}, Grade: {student.GetAssignedGrades(course)}");
                });
        }
    }

    public static void GetStudentFromCourse(Course? course)
    {
        ValidationHelper.ValidateNotNull(course, "Course");

        var enrolledStudents = course.GetEnrolledStudents();
        if (enrolledStudents == null || !enrolledStudents.Any())
        {
            Console.WriteLine($"No students enrolled in {course.GetCourseName()}.");
            return;
        }

        Console.WriteLine($"Students enrolled in {course.GetCourseName()}:");
        ListEntities(enrolledStudents.OfType<Student>(), "Student",
            student =>
            {
                Console.WriteLine($"Student ID: {student.GetStudentId()}, Name: {student.GetStudentFullName()}");
            });
    }

    public static void ListStudentsInCourses(IEnumerable<Course?>? courses, IEnumerable<Student?> students, IUser user)
    {
        ValidationHelper.ValidateNotNull(courses, "Courses list cannot be null.");
        ValidationHelper.ValidateNotNull(students, "Students list cannot be null.");
        ValidationHelper.ValidateUserPermission(user, true);

        foreach (var course in courses.OfType<Course>())
        {
            Console.WriteLine($"Course: {course.GetCourseName()} (ID: {course.GetCourseId()})");

            var enrolledStudents = course.GetEnrolledStudents();
            if (enrolledStudents == null || !enrolledStudents.Any())
            {
                Console.WriteLine("  No students enrolled in this course.");
                continue;
            }

            ListEntities(enrolledStudents.OfType<Student>(), "Student",
                student =>
                {
                    Console.WriteLine($"  Student ID: {student.GetStudentId()}, Name: {student.GetStudentFullName()}");
                });
        }
    }

    public static void UpdateCourseId(Course? course)
    {
        ValidationHelper.ValidateNotNull(course, "Course");
        Console.WriteLine("Enter new Course ID:");
        var newCourseId = InputHelper.GetValidatedIntInput("New Course ID:");
        course.SetCourseId(newCourseId);
        Console.WriteLine("Course ID updated successfully.");
    }

    public static void UpdateCourseName(Course? course)
    {
        ValidationHelper.ValidateNotNull(course, "Course");
        Console.WriteLine("Enter new Course Name:");
        var newCourseName = InputHelper.GetValidatedStringInput("New Course Name:");
        course.SetCourseName(newCourseName);
        Console.WriteLine("Course Name updated successfully.");
    }

    public static void DisplayTotalCourses(IEnumerable<Course?>? courses)
    {
        ValidationHelper.ValidateNotNull(courses, "Courses");

        var courseList = courses.OfType<Course>().ToList();
        if (!courseList.Any())
        {
            Console.WriteLine("No courses available.");
            return;
        }

        Console.WriteLine($"Total number of courses: {courseList.Count}");
    }

    public static void DisplayCourseActions(Course? course, object user)
    {
        ValidationHelper.ValidateNotNull(course, "Course");
        ValidationHelper.ValidateUserPermission(user, true);

        Console.WriteLine("Do you want to:");
        Console.WriteLine("1. Update Course ID");
        Console.WriteLine("2. Update Course Name");
        Console.WriteLine("3. Assign Courses To Students");
        Console.WriteLine("4. Exit");

        var choice = InputHelper.GetValidatedIntInput("Enter your choice (1-4):");

        switch (choice)
        {
            case 1:
                UpdateCourseId(course);
                break;
            case 2:
                UpdateCourseName(course);
                break;
            case 3:
                var schoolHelper = new SchoolHelper(); 
                SchoolHandler.AssignCoursesToStudents(schoolHelper, new List<Course>(), new List<Student?>(), (IUser)user);
                break;
            case 4:
                return;
            default:
                Console.WriteLine("Invalid choice. Please select 1, 2, 3, or 4.");
                break;
        }
    }

    private static void ListEntities<T>(IEnumerable<T?> entities, string entityType, Action<T> displayAction)
        where T : class
    {
        try
        {
            var enumerable = entities.ToList();
            if (entities == null || !enumerable.Any())
            {
                throw new Exceptions.EntityNotFoundException($"No {entityType}s found.");
            }

            foreach (var entity in enumerable)
            {
                if (entity == null)
                {
                    Console.WriteLine($"Invalid {entityType} detected.");
                    continue;
                }

                displayAction(entity);
            }
        }
        catch (Exceptions.EntityNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while listing entities: {ex.Message}");
        }
    }
}