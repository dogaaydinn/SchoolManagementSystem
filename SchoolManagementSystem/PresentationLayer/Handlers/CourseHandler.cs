using SchoolManagementSystem.BusinessLogicLayer.Validations;
using SchoolManagementSystem.Interfaces.User;
using SchoolManagementSystem.Models.Concrete;

namespace SchoolManagementSystem.PresentationLayer.Handlers;

public static class CourseHandler
{
    public static void DisplayCourseDetails(IEnumerable<Course> courses, object? user)
    {
        ValidationHelper.ValidateNotNull(courses, "Courses");
        ValidationHelper.ValidateUserPermission(user, requireTeacherOrAdmin: false);

        Console.WriteLine("Do you want to search by:");
        Console.WriteLine("1. Course ID");
        Console.WriteLine("2. Course Name");
        Console.WriteLine("3. List all courses");

        var choice = InputHelper.GetValidatedIntInput("Enter your choice (1-3):");
        Course? course = null;

        switch (choice)
        {
            case 1:
                course = new CourseHelper().GetCourseById(courses.ToList());
                break;
            case 2:
                course = new CourseHelper().GetCourseByName(courses.ToList());
                break;
            case 3:
                DisplayCourseNames(courses, user);
                return; 
            default:
                Console.WriteLine("Invalid choice. Please select 1, 2, or 3.");
                return;
        }

        if (course != null)
        {
            DisplayCourseInfo(course);
        }
        else
        {
            PromptAndDisplayAllCourses(courses, user);
        }
    }

    private static void PromptAndDisplayAllCourses(IEnumerable<Course> courses, object? user)
    {
        Console.WriteLine("Course not found. Would you like to see the list of courses? (yes/no)");
        if (Console.ReadLine()?.Trim().ToLower() == "yes")
        {
            DisplayCourseNames(courses, user);
        }
    }

    private static void DisplayCourseInfo(Course course)
    {
        Console.WriteLine($"Course ID: {course.GetCourseId()}, Name: {course.GetCourseName()}, " +
                          $"Teacher: {course.GetAssignedTeacherName()}, " +
                          $"Enrolled Students: {course.GetEnrolledStudents()?.Count ?? 0}, " +
                          $"Credits: {course.GetCredits()}");
    }

    public static void DisplayCourseNames(IEnumerable<Course> courses, object? user)
    {
        Console.WriteLine("Listing all courses:");
        foreach (var course in courses)
        {
            Console.WriteLine($"Course ID: {course.GetCourseId()}, Name: {course.GetCourseName()}");
        }
    }
    public static void ListStudentsInCourses(List<Course> courses, IUser user)
    {
        var schoolHelper = new SchoolHelper();
        
        schoolHelper.DisplayCourses(courses);

        foreach (var course in courses)
        {
            Console.WriteLine($"\nCourse: {course.GetCourseName()} (ID: {course.GetCourseId()})");
            var enrolledStudents = course.GetEnrolledStudents();
            if (enrolledStudents == null || !enrolledStudents.Any())
            {
                Console.WriteLine("No students enrolled in this course.");
                continue;
            }

            schoolHelper.DisplayStudents(enrolledStudents.OfType<Student?>().ToList());
        }
    }
    
    public static void DisplayCourseActions(Course course, object? user)
    {
        ValidationHelper.ValidateNotNull(course, "Course");
        ValidationHelper.ValidateUserPermission(user, requireTeacherOrAdmin: true);

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
                AssignCoursesToStudents(course);
                break;
            case 4:
                return;
            default:
                Console.WriteLine("Invalid choice. Please select 1, 2, 3, or 4.");
                break;
        }
    }
}
